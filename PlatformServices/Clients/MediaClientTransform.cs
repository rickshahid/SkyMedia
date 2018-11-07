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

        private Preset GetAnalyzerPreset(bool audioOnly)
        {
            Preset analyzerPreset;
            if (audioOnly)
            {
                analyzerPreset = new AudioAnalyzerPreset();
            }
            else
            {
                analyzerPreset = new VideoAnalyzerPreset();
            }
            return analyzerPreset;
        }

        private TransformOutput GetTransformOutput(Preset transformPreset, MediaTransformOutput transformOutput)
        {
            return new TransformOutput(transformPreset)
            {
                RelativePriority = transformOutput.RelativePriority,
                OnError = transformOutput.OnError
            };
        }

        public Transform CreateTransform(bool adaptiveStreaming, bool thumbnailImages, bool videoAnalyzer, bool audioAnalyzer, bool videoIndexer, bool audioIndexer)
        {
            MediaTransformPreset transformPreset = new MediaTransformPreset();
            if (adaptiveStreaming)
            {
                transformPreset = transformPreset | MediaTransformPreset.AdaptiveStreaming;
            }
            if (thumbnailImages)
            {
                transformPreset = transformPreset | MediaTransformPreset.ThumbnailImages;
            }
            if (videoAnalyzer)
            {
                transformPreset = transformPreset | MediaTransformPreset.VideoAnalyzer;
            }
            if (audioAnalyzer)
            {
                transformPreset = transformPreset | MediaTransformPreset.AudioAnalyzer;
            }
            if (videoIndexer)
            {
                transformPreset = transformPreset | MediaTransformPreset.VideoIndexer;
            }
            if (audioIndexer)
            {
                transformPreset = transformPreset | MediaTransformPreset.AudioIndexer;
            }
            return CreateTransform(transformPreset);
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
                    case MediaTransformPreset.ThumbnailImages:
                        transformName = GetTransformName(defaultName, transformName, transformOutput);
                        StandardEncoderPreset thumbnailPreset = GetThumbnailPreset(null);
                        transformOutputPreset = GetTransformOutput(thumbnailPreset, transformOutput);
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

        public Transform CreateTransform(MediaTransformPreset transformPreset)
        {
            List<MediaTransformOutput> transformOutputs = new List<MediaTransformOutput>();
            if (transformPreset.HasFlag(MediaTransformPreset.AdaptiveStreaming))
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    TransformPreset = MediaTransformPreset.AdaptiveStreaming,
                    RelativePriority = Priority.High,
                    OnError = OnErrorType.StopProcessingJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (transformPreset.HasFlag(MediaTransformPreset.ThumbnailImages))
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    TransformPreset = MediaTransformPreset.ThumbnailImages,
                    RelativePriority = Priority.Normal,
                    OnError = OnErrorType.ContinueJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (transformPreset.HasFlag(MediaTransformPreset.VideoAnalyzer))
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    TransformPreset = MediaTransformPreset.VideoAnalyzer,
                    RelativePriority = Priority.Normal,
                    OnError = OnErrorType.ContinueJob
                };
                transformOutputs.Add(transformOutput);
            }
            if (transformPreset.HasFlag(MediaTransformPreset.AudioAnalyzer))
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

            MediaTransformPreset transformPreset = MediaTransformPreset.AdaptiveStreaming;
            Transform transform = CreateTransform(transformPreset);
            transforms.Add(transform);

            transformPreset = MediaTransformPreset.AdaptiveStreaming | MediaTransformPreset.ThumbnailImages;
            transform = CreateTransform(transformPreset);
            transforms.Add(transform);

            transformPreset = MediaTransformPreset.AdaptiveStreaming | MediaTransformPreset.ThumbnailImages | MediaTransformPreset.VideoAnalyzer;
            transform = CreateTransform(transformPreset);
            transforms.Add(transform);

            transformPreset = MediaTransformPreset.AdaptiveStreaming | MediaTransformPreset.ThumbnailImages | MediaTransformPreset.AudioAnalyzer;
            transform = CreateTransform(transformPreset);
            transforms.Add(transform);

            transformPreset = MediaTransformPreset.AdaptiveStreaming | MediaTransformPreset.VideoAnalyzer;
            transform = CreateTransform(transformPreset);
            transforms.Add(transform);

            transformPreset = MediaTransformPreset.AdaptiveStreaming | MediaTransformPreset.AudioAnalyzer;
            transform = CreateTransform(transformPreset);
            transforms.Add(transform);

            transformPreset = MediaTransformPreset.ThumbnailImages;
            transform = CreateTransform(transformPreset);
            transforms.Add(transform);

            transformPreset = MediaTransformPreset.ThumbnailImages | MediaTransformPreset.VideoAnalyzer;
            transform = CreateTransform(transformPreset);
            transforms.Add(transform);

            transformPreset = MediaTransformPreset.ThumbnailImages | MediaTransformPreset.AudioAnalyzer;
            transform = CreateTransform(transformPreset);
            transforms.Add(transform);

            transformPreset = MediaTransformPreset.VideoAnalyzer;
            transform = CreateTransform(transformPreset);
            transforms.Add(transform);

            transformPreset = MediaTransformPreset.AudioAnalyzer;
            transform = CreateTransform(transformPreset);
            transforms.Add(transform);

            return transforms.ToArray();
        }
    }
}