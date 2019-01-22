using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private string GetTransformName(string transformName, MediaTransformOutput[] transformOutputs)
        {
            if (string.IsNullOrEmpty(transformName))
            {
                foreach (MediaTransformOutput transformOutput in transformOutputs)
                {
                    if (!string.IsNullOrEmpty(transformName))
                    {
                        transformName = string.Concat(transformName, Constant.Media.Transform.Preset.NameDelimiter);
                    }
                    transformName = string.Concat(transformName, transformOutput.PresetName);
                }
            }
            return transformName;
        }

        private StandardEncoderPreset GetThumbnailPreset(int? thumbnailSpriteColumns)
        {
            StandardEncoderPreset thumbnailPreset = new StandardEncoderPreset()
            {
                Codecs = new Codec[]
                {
                    new JpgImage()
                    {
                        Start = "0%",
                        Step = "2%",
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
                        FilenamePattern = string.Concat(Constant.Media.Thumbnail.FileNamePrefix, Constant.Media.Thumbnail.FileNameSuffix)
                    }
                }
            };
            return thumbnailPreset;
        }

        private TransformOutput GetTransformOutput(MediaTransformOutput transformOutput, int? thumbnailSpriteColumns)
        {
            Preset transformPreset = null;
            switch (transformOutput.PresetType)
            {
                case MediaTransformPreset.MediaEncoder:
                    EncoderNamedPreset presetName = EncoderNamedPreset.AdaptiveStreaming; 
                    if (!string.IsNullOrEmpty(transformOutput.PresetName))
                    {
                        presetName = transformOutput.PresetName;
                    }
                    transformPreset = new BuiltInStandardEncoderPreset(presetName);
                    break;
                case MediaTransformPreset.ThumbnailImages:
                    transformPreset = GetThumbnailPreset(thumbnailSpriteColumns);
                    break;
                case MediaTransformPreset.VideoAnalyzer:
                    transformPreset = new VideoAnalyzerPreset();
                    break;
                case MediaTransformPreset.AudioAnalyzer:
                    transformPreset = new AudioAnalyzerPreset();
                    break;
            }
            if (transformPreset == null)
            {
                return null;
            }
            else
            {
                return new TransformOutput(transformPreset)
                {
                    RelativePriority = transformOutput.RelativePriority,
                    OnError = transformOutput.OnError
                };
            }
        }

        public Transform CreateTransform(string transformName, string transformDescription, MediaTransformOutput[] transformOutputs, int? thumbnailSpriteColumns)
        {
            Transform transform = null;
            List<TransformOutput> transformOutputList = new List<TransformOutput>();
            foreach (MediaTransformOutput transformOutput in transformOutputs)
            {
                TransformOutput transformOutputItem = GetTransformOutput(transformOutput, thumbnailSpriteColumns);
                if (transformOutputItem != null)
                {
                    transformOutputList.Add(transformOutputItem);
                }
            }
            if (transformOutputList.Count > 0)
            {
                transformName = GetTransformName(transformName, transformOutputs);
                transform = _media.Transforms.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, transformOutputList.ToArray(), transformDescription);
            }
            return transform;
        }

        public Transform CreateTransform(bool adaptiveStreaming, bool thumbnailImages, bool videoAnalyzer, bool audioAnalyzer, bool videoIndexer, bool audioIndexer)
        {
            List<MediaTransformOutput> transformOutputs = new List<MediaTransformOutput>();
            if (adaptiveStreaming)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.MediaEncoder,
                    PresetName = EncoderNamedPreset.AdaptiveStreaming.ToString()
                };
                transformOutputs.Add(transformOutput);
            }
            if (thumbnailImages)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.ThumbnailImages,
                    PresetName = MediaTransformPreset.ThumbnailImages.ToString()
                };
                transformOutputs.Add(transformOutput);
            }
            if (videoAnalyzer)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.VideoAnalyzer,
                    PresetName = MediaTransformPreset.VideoAnalyzer.ToString()
                };
                transformOutputs.Add(transformOutput);
            }
            if (audioAnalyzer)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.AudioAnalyzer,
                    PresetName = MediaTransformPreset.AudioAnalyzer.ToString()
                };
                transformOutputs.Add(transformOutput);
            }
            if (videoIndexer)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.VideoIndexer,
                    PresetName = MediaTransformPreset.VideoIndexer.ToString()
                };
                transformOutputs.Add(transformOutput);
            }
            if (audioIndexer)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.AudioIndexer,
                    PresetName = MediaTransformPreset.AudioIndexer.ToString()
                };
                transformOutputs.Add(transformOutput);
            }
            return CreateTransform(null, null, transformOutputs.ToArray(), null);
        }

        public Transform CreateTransform(MediaTransformPreset[] transformPresets)
        {
            List<MediaTransformOutput> transformOutputs = new List<MediaTransformOutput>();
            foreach (MediaTransformPreset transformPreset in transformPresets)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = transformPreset
                };
                transformOutputs.Add(transformOutput);
            }
            return CreateTransform(null, null, transformOutputs.ToArray(), null);
        }

        public Transform[] CreateTransforms()
        {
            List<Transform> transforms = new List<Transform>();

            Transform transform = CreateTransform(true, false, false, false, false, false);
            transforms.Add(transform);

            transform = CreateTransform(true, true, false, false, false, false);
            transforms.Add(transform);

            transform = CreateTransform(true, true, true, false, false, false);
            transforms.Add(transform);

            transform = CreateTransform(true, true, false, true, false, false);
            transforms.Add(transform);

            transform = CreateTransform(true, false, true, false, false, false);
            transforms.Add(transform);

            transform = CreateTransform(true, false, false, true, false, false);
            transforms.Add(transform);

            transform = CreateTransform(false, true, false, false, false, false);
            transforms.Add(transform);

            transform = CreateTransform(false, true, true, false, false, false);
            transforms.Add(transform);

            transform = CreateTransform(false, true, false, true, false, false);
            transforms.Add(transform);

            transform = CreateTransform(false, false, true, false, false, false);
            transforms.Add(transform);

            transform = CreateTransform(false, false, false, true, false, false);
            transforms.Add(transform);

            return transforms.ToArray();
        }
    }
}