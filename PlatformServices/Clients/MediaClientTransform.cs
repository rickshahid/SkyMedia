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
                transformName = string.Concat(transformName, transformOutput.TransformPreset.ToString());
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

        public MediaTransformPresets GetTransformPresets(MediaTransformPreset[] transformPresets)
        {
            MediaTransformPresets presets = new MediaTransformPresets();
            foreach (MediaTransformPreset transformPreset in transformPresets)
            {
                switch (transformPreset)
                {
                    case MediaTransformPreset.AdaptiveStreaming:
                        presets.AdaptiveStreaming = true;
                        break;
                    case MediaTransformPreset.ThumbnailSprite:
                        presets.ThumbnailSprite = true;
                        break;
                    case MediaTransformPreset.VideoAnalyzer:
                        presets.VideoAnalyzer = true;
                        break;
                    case MediaTransformPreset.AudioAnalyzer:
                        presets.AudioAnalyzer = true;
                        break;
                    case MediaTransformPreset.VideoIndexer:
                        presets.VideoIndexer = true;
                        break;
                    case MediaTransformPreset.AudioIndexer:
                        presets.AudioIndexer = true;
                        break;
                }
            }
            return presets;
        }

        public Transform CreateTransform(string transformName, string transformDescription, MediaTransformOutput[] transformOutputs)
        {
            Transform transform = null;
            bool defaultName = string.IsNullOrEmpty(transformName);
            List<TransformOutput> transformOutputPresets = new List<TransformOutput>();
            foreach (MediaTransformOutput transformOutput in transformOutputs)
            {
                TransformOutput transformOutputPreset;
                switch (transformOutput.TransformPreset)
                {
                    case MediaTransformPreset.AdaptiveStreaming:
                        transformName = GetTransformName(defaultName, transformName, transformOutput);
                        BuiltInStandardEncoderPreset standardEncoderPreset = new BuiltInStandardEncoderPreset(EncoderNamedPreset.AdaptiveStreaming);
                        transformOutputPreset = GetTransformOutput(standardEncoderPreset, transformOutput);
                        transformOutputPresets.Add(transformOutputPreset);
                        break;
                    case MediaTransformPreset.ThumbnailSprite:
                        transformName = GetTransformName(defaultName, transformName, transformOutput);
                        StandardEncoderPreset thumbnailSpritePreset = GetThumbnailPreset(Constant.Media.Thumbnail.SpriteColumns);
                        transformOutputPreset = GetTransformOutput(thumbnailSpritePreset, transformOutput);
                        transformOutputPresets.Add(transformOutputPreset);
                        break;
                    case MediaTransformPreset.VideoAnalyzer:
                        transformName = GetTransformName(defaultName, transformName, transformOutput);
                        VideoAnalyzerPreset videoAnalyzerPreset = (VideoAnalyzerPreset)GetAnalyzerPreset(false);
                        transformOutputPreset = GetTransformOutput(videoAnalyzerPreset, transformOutput);
                        transformOutputPresets.Add(transformOutputPreset);
                        break;
                    case MediaTransformPreset.AudioAnalyzer:
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

        public Transform CreateTransform(MediaTransformPresets transformPresets)
        {
            List<MediaTransformOutput> transformOutputs = new List<MediaTransformOutput>();
            if (transformPresets.AdaptiveStreaming)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    TransformPreset = MediaTransformPreset.AdaptiveStreaming,
                    RelativePriority = Priority.High,
                    OnError = OnErrorType.StopProcessingJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (transformPresets.ThumbnailSprite)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    TransformPreset = MediaTransformPreset.ThumbnailSprite,
                    RelativePriority = Priority.Normal,
                    OnError = OnErrorType.ContinueJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (transformPresets.VideoAnalyzer)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    TransformPreset = MediaTransformPreset.VideoAnalyzer,
                    RelativePriority = Priority.Normal,
                    OnError = OnErrorType.ContinueJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (transformPresets.AudioAnalyzer)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    TransformPreset = MediaTransformPreset.AudioAnalyzer,
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

            MediaTransformPresets transformPresets = new MediaTransformPresets()
            {
                AdaptiveStreaming = true
            };
            Transform transform = CreateTransform(transformPresets);
            transforms.Add(transform);

            transformPresets = new MediaTransformPresets()
            {
                AdaptiveStreaming = true,
                ThumbnailSprite = true
            };
            transform = CreateTransform(transformPresets);
            transforms.Add(transform);

            transformPresets = new MediaTransformPresets()
            {
                AdaptiveStreaming = true,
                VideoAnalyzer = true
            };
            transform = CreateTransform(transformPresets);
            transforms.Add(transform);

            transformPresets = new MediaTransformPresets()
            {
                AdaptiveStreaming = true,
                AudioIndexer = true
            };
            transform = CreateTransform(transformPresets);
            transforms.Add(transform);

            transformPresets = new MediaTransformPresets()
            {
                AdaptiveStreaming = true,
                ThumbnailSprite = true,
                VideoAnalyzer = true
            };
            transform = CreateTransform(transformPresets);
            transforms.Add(transform);

            transformPresets = new MediaTransformPresets()
            {
                AdaptiveStreaming = true,
                ThumbnailSprite = true,
                AudioAnalyzer = true
            };
            transform = CreateTransform(transformPresets);
            transforms.Add(transform);

            transformPresets = new MediaTransformPresets()
            {
                ThumbnailSprite = true,
            };
            transform = CreateTransform(transformPresets);
            transforms.Add(transform);

            transformPresets = new MediaTransformPresets()
            {
                ThumbnailSprite = true,
                VideoAnalyzer = true
            };
            transform = CreateTransform(transformPresets);
            transforms.Add(transform);

            transformPresets = new MediaTransformPresets()
            {
                ThumbnailSprite = true,
                AudioAnalyzer = true
            };
            transform = CreateTransform(transformPresets);
            transforms.Add(transform);

            transformPresets = new MediaTransformPresets()
            {
                VideoAnalyzer = true
            };
            transform = CreateTransform(transformPresets);
            transforms.Add(transform);

            transformPresets = new MediaTransformPresets()
            {
                AudioAnalyzer = true
            };
            transform = CreateTransform(transformPresets);
            transforms.Add(transform);

            return transforms.ToArray();
        }
    }
}