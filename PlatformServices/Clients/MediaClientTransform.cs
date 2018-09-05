using System;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private string GetTransformName(bool defaultName, string transformName, MediaTransformOutput transformOutput)
        {
            if (defaultName)
            {
                if (!string.IsNullOrEmpty(transformName))
                {
                    transformName = string.Concat(transformName, Constant.Media.Transform.Preset.NameDelimiter);
                }
                transformName = string.Concat(transformName, transformOutput.PresetName);
            }
            return transformName;
        }

        private StandardEncoderPreset GetThumbnailPreset(int? spriteColumns)
        {
            StandardEncoderPreset thumbnailPreset = new StandardEncoderPreset()
            {
                Codecs = new Codec[]
                {
                    new JpgImage()
                    {
                        Start = "0%",
                        Step = "1%",
                        Range = "100%",
                        Layers = new JpgLayer[]
                        {
                            new JpgLayer()
                            {
                                Height = "10%",
                                Width = "10%",
                                Quality = 90
                            }
                        }
                    }
                },
                Formats = new Format[]
                {
                    new JpgFormat()
                    {
                        FilenamePattern = "Thumbnail-{Basename}-{Index}{Extension}"
                    }
                }
            };
            return thumbnailPreset;
        }

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

        public Transform CreateTransform(string transformName, string transformDescription, MediaTransformOutput[] transformOutputs)
        {
            Transform transform = null;
            bool defaultName = string.IsNullOrEmpty(transformName);
            List<TransformOutput> transformOutputPresets = new List<TransformOutput>();
            foreach (MediaTransformOutput transformOutput in transformOutputs)
            {
                TransformOutput transformOutputPreset;
                switch (transformOutput.PresetProcessor)
                {
                    case MediaProcessor.StandardEncoder:
                        transformName = GetTransformName(defaultName, transformName, transformOutput);
                        if (string.Equals(transformOutput.PresetName, Constant.Media.Transform.Preset.ThumbnailSprite, StringComparison.OrdinalIgnoreCase)) {

                            StandardEncoderPreset thumbnailSpritePreset = GetThumbnailPreset(Constant.Media.Thumbnail.SpriteColumns);
                            transformOutputPreset = GetTransformOutput(thumbnailSpritePreset, transformOutput);
                        }
                        else
                        {
                            BuiltInStandardEncoderPreset standardEncoderPreset = new BuiltInStandardEncoderPreset(transformOutput.PresetName);
                            transformOutputPreset = GetTransformOutput(standardEncoderPreset, transformOutput);
                        }
                        transformOutputPresets.Add(transformOutputPreset);
                        break;
                    case MediaProcessor.VideoAnalyzer:
                        transformName = GetTransformName(defaultName, transformName, transformOutput);
                        VideoAnalyzerPreset videoAnalyzerPreset = (VideoAnalyzerPreset)GetAnalyzerPreset(false);
                        transformOutputPreset = GetTransformOutput(videoAnalyzerPreset, transformOutput);
                        transformOutputPresets.Add(transformOutputPreset);
                        break;
                    case MediaProcessor.AudioAnalyzer:
                        transformName = GetTransformName(defaultName, transformName, transformOutput);
                        AudioAnalyzerPreset audioAnalyzerPreset = (AudioAnalyzerPreset)GetAnalyzerPreset(true);
                        transformOutputPreset = GetTransformOutput(audioAnalyzerPreset, transformOutput);
                        transformOutputPresets.Add(transformOutputPreset);
                        break;
                }
            }
            if (transformOutputPresets.Count > 0)
            {
                transform = _media.Transforms.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, transformOutputPresets.ToArray(), transformDescription);
            }
            return transform;
        }

        public Transform CreateTransform(bool adaptiveStreaming, bool thumbnailSprite, bool videoAnalyzer, bool audioAnalyzer)
        {
            List<MediaTransformOutput> transformOutputs = new List<MediaTransformOutput>();
            if (adaptiveStreaming)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetProcessor = MediaProcessor.StandardEncoder,
                    PresetName = EncoderNamedPreset.AdaptiveStreaming,
                    RelativePriority = Priority.High,
                    OnError = OnErrorType.StopProcessingJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (thumbnailSprite)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetProcessor = MediaProcessor.StandardEncoder,
                    PresetName = Constant.Media.Transform.Preset.ThumbnailSprite,
                    RelativePriority = Priority.Normal,
                    OnError = OnErrorType.ContinueJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (videoAnalyzer)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetProcessor = MediaProcessor.VideoAnalyzer,
                    PresetName = Constant.Media.Transform.Preset.VideoAnalyzer,
                    RelativePriority = Priority.Normal,
                    OnError = OnErrorType.ContinueJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (audioAnalyzer)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetProcessor = MediaProcessor.AudioAnalyzer,
                    PresetName = Constant.Media.Transform.Preset.AudioAnalyzer,
                    RelativePriority = Priority.Normal,
                    OnError = OnErrorType.ContinueJob
                };
                transformOutputs.Add(transformOutput);
            }
            return CreateTransform(null, null, transformOutputs.ToArray());
        }

        public Transform[] CreateTransforms()
        {
            List<Transform> transforms = new List<Transform>();
            Transform transform = CreateTransform(true, false, false, false);
            transforms.Add(transform);
            transform = CreateTransform(true, true, false, false);
            transforms.Add(transform);
            transform = CreateTransform(true, false, true, false);
            transforms.Add(transform);
            transform = CreateTransform(true, false, false, true);
            transforms.Add(transform);
            transform = CreateTransform(true, true, true, false);
            transforms.Add(transform);
            transform = CreateTransform(true, true, false, true);
            transforms.Add(transform);
            transform = CreateTransform(false, true, false, false);
            transforms.Add(transform);
            transform = CreateTransform(false, true, true, false);
            transforms.Add(transform);
            transform = CreateTransform(false, true, false, true);
            transforms.Add(transform);
            transform = CreateTransform(false, false, true, false);
            transforms.Add(transform);
            transform = CreateTransform(false, false, false, true);
            transforms.Add(transform);
            return transforms.ToArray();
        }
    }
}