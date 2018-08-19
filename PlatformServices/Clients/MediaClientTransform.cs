using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private Preset GetAnalyzerPreset(bool audioOnly)
        {
            Preset analyzerPreset;
            if (audioOnly)
            {
                analyzerPreset = new AudioAnalyzerPreset();
            }
            else
            {
                analyzerPreset = new VideoAnalyzerPreset()
                {
                    AudioInsightsOnly = false
                };
            }
            return analyzerPreset;
        }

        private TransformOutput GetTransformOutput(Preset transformPreset, MediaTransformOutput transformOutput)
        {
            return new TransformOutput(transformPreset)
            {
                RelativePriority = (Priority)transformOutput.RelativePriority,
                OnError = (OnErrorType)transformOutput.OnError
            };
        }

        private Transform CreateTransform(string transformName, string transformDescription, TransformOutput[] transformOutputs)
        {
            return _media.Transforms.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, transformOutputs, transformDescription);
        }

        public Transform CreateTransform(string transformName, string transformDescription, MediaTransformOutput[] transformOutputs)
        {
            Transform transform = null;
            bool defaultName = string.IsNullOrEmpty(transformName);
            List<TransformOutput> transformOutputList = new List<TransformOutput>();
            if (transformOutputs[0].PresetEnabled)
            {
                if (defaultName)
                {
                    transformName = transformOutputs[0].PresetName;
                }
                BuiltInStandardEncoderPreset encoderPreset = new BuiltInStandardEncoderPreset(transformOutputs[0].PresetName);
                TransformOutput transformOutput = GetTransformOutput(encoderPreset, transformOutputs[0]);
                transformOutputList.Add(transformOutput);
                if (transformOutputs[1].PresetEnabled)
                {
                    if (defaultName)
                    {
                        transformName = string.Concat(transformName, Constant.Media.Transform.PresetNameDelimiter, Constant.Media.Transform.PresetNameAnalyzerVideo);
                    }
                    VideoAnalyzerPreset analyzerPreset = (VideoAnalyzerPreset)GetAnalyzerPreset(false);
                    transformOutput = GetTransformOutput(analyzerPreset, transformOutputs[1]);
                    transformOutputList.Add(transformOutput);
                }
                else if (transformOutputs[2].PresetEnabled)
                {
                    if (defaultName)
                    {
                        transformName = string.Concat(transformName, Constant.Media.Transform.PresetNameDelimiter, Constant.Media.Transform.PresetNameAnalyzerAudio);
                    }
                    AudioAnalyzerPreset analyzerPreset = (AudioAnalyzerPreset)GetAnalyzerPreset(true);
                    transformOutput = GetTransformOutput(analyzerPreset, transformOutputs[2]);
                    transformOutputList.Add(transformOutput);
                }
            }
            else if (transformOutputs[1].PresetEnabled)
            {
                if (defaultName)
                {
                    transformName = Constant.Media.Transform.PresetNameAnalyzerVideo;
                }
                VideoAnalyzerPreset analyzerPreset = (VideoAnalyzerPreset)GetAnalyzerPreset(false);
                TransformOutput transformOutput = GetTransformOutput(analyzerPreset, transformOutputs[1]);
                transformOutputList.Add(transformOutput);
            }
            else if (transformOutputs[2].PresetEnabled)
            {
                if (defaultName)
                {
                    transformName = Constant.Media.Transform.PresetNameAnalyzerAudio;
                }
                AudioAnalyzerPreset analyzerPreset = (AudioAnalyzerPreset)GetAnalyzerPreset(true);
                TransformOutput transformOutput = GetTransformOutput(analyzerPreset, transformOutputs[2]);
                transformOutputList.Add(transformOutput);
            }
            if (transformOutputList.Count > 0)
            {
                transform = CreateTransform(transformName, transformDescription, transformOutputList.ToArray());
            }
            return transform;
        }

        public Transform CreateTransform(bool standardEncoderPreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset)
        {
            if (IndexerIsEnabled())
            {
                videoAnalyzerPreset = false;
                audioAnalyzerPreset = false;
            }
            MediaTransformOutput standardEncoderOutput = new MediaTransformOutput()
            {
                PresetEnabled = standardEncoderPreset,
                PresetName = EncoderNamedPreset.AdaptiveStreaming,
                RelativePriority = Priority.Normal,
                OnError = OnErrorType.ContinueJob
            };
            MediaTransformOutput videoAnalyzerOutput = new MediaTransformOutput()
            {
                PresetEnabled = videoAnalyzerPreset,
                RelativePriority = Priority.Normal,
                OnError = OnErrorType.ContinueJob
            };
            MediaTransformOutput audioAnalyzerOutput = new MediaTransformOutput()
            {
                PresetEnabled = audioAnalyzerPreset,
                RelativePriority = Priority.Normal,
                OnError = OnErrorType.ContinueJob
            };
            MediaTransformOutput[] transformOutputs = new MediaTransformOutput[] { standardEncoderOutput, videoAnalyzerOutput, audioAnalyzerOutput };
            return CreateTransform(null, null, transformOutputs);
        }
    }
}