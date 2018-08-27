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
                    transformName = string.Concat(transformName, Constant.Media.Transform.PresetNameDelimiter);
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
            bool defaultName = string.IsNullOrEmpty(transformName);
            List<TransformOutput> transformOutputList = new List<TransformOutput>();
            foreach (MediaTransformOutput transformOutput in transformOutputs)
            {
                switch (transformOutput.PresetType)
                {
                    case MediaTransformPreset.StandardEncoder:
                        transformName = GetTransformName(defaultName, transformName, transformOutput);
                        BuiltInStandardEncoderPreset standardEncoderPreset = new BuiltInStandardEncoderPreset(transformOutput.PresetName);
                        TransformOutput transformOutputItem = GetTransformOutput(standardEncoderPreset, transformOutput);
                        transformOutputList.Add(transformOutputItem);
                        break;
                    case MediaTransformPreset.ThumbnailSprite:
                        transformName = GetTransformName(defaultName, transformName, transformOutput);
                        StandardEncoderPreset thumbnailSpritePreset = GetThumbnailPreset(Constant.Media.Thumbnail.SpriteColumns);
                        transformOutputItem = GetTransformOutput(thumbnailSpritePreset, transformOutput);
                        transformOutputList.Add(transformOutputItem);
                        break;
                    case MediaTransformPreset.VideoAnalyzer:
                        transformName = GetTransformName(defaultName, transformName, transformOutput);
                        VideoAnalyzerPreset videoAnalyzerPreset = (VideoAnalyzerPreset)GetAnalyzerPreset(false);
                        transformOutputItem = GetTransformOutput(videoAnalyzerPreset, transformOutput);
                        transformOutputList.Add(transformOutputItem);
                        break;
                    case MediaTransformPreset.AudioAnalyzer:
                        transformName = GetTransformName(defaultName, transformName, transformOutput);
                        AudioAnalyzerPreset audioAnalyzerPreset = (AudioAnalyzerPreset)GetAnalyzerPreset(true);
                        transformOutputItem = GetTransformOutput(audioAnalyzerPreset, transformOutput);
                        transformOutputList.Add(transformOutputItem);
                        break;
                }
            }
            return _media.Transforms.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, transformOutputList.ToArray(), transformDescription);
        }

        public Transform CreateTransform(bool standardEncoderPreset, bool thumbnailSpritePreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset)
        {
            if (IndexerIsEnabled())
            {
                videoAnalyzerPreset = false;
                audioAnalyzerPreset = false;
            }
            List<MediaTransformOutput> transformOutputs = new List<MediaTransformOutput>();
            if (standardEncoderPreset)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.StandardEncoder,
                    PresetName = EncoderNamedPreset.AdaptiveStreaming,
                    RelativePriority = Priority.Normal,
                    OnError = OnErrorType.ContinueJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (thumbnailSpritePreset)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.ThumbnailSprite,
                    PresetName = MediaTransformPreset.ThumbnailSprite.ToString(),
                    RelativePriority = Priority.Normal,
                    OnError = OnErrorType.ContinueJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (videoAnalyzerPreset)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.VideoAnalyzer,
                    PresetName = MediaTransformPreset.VideoAnalyzer.ToString(),
                    RelativePriority = Priority.Normal,
                    OnError = OnErrorType.ContinueJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (audioAnalyzerPreset)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.AudioAnalyzer,
                    PresetName = MediaTransformPreset.AudioAnalyzer.ToString(),
                    RelativePriority = Priority.Normal,
                    OnError = OnErrorType.ContinueJob
                };
                transformOutputs.Add(transformOutput);
            }
            return CreateTransform(null, null, transformOutputs.ToArray());
        }

        public void CreateTransforms()
        {
            CreateTransform(true, false, false, false);
            CreateTransform(true, true, false, false);
            CreateTransform(true, false, true, false);
            CreateTransform(true, false, false, true);
            CreateTransform(false, true, false, false);
            CreateTransform(false, true, true, false);
            CreateTransform(false, true, false, true);
            CreateTransform(false, false, true, false);
            CreateTransform(false, false, false, true);
        }
    }
}