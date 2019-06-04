using System.Linq;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private MediaTransformPreset[] GetTransformPresets(string transformName)
        {
            List<MediaTransformPreset> transformPresets = new List<MediaTransformPreset>();
            Transform transform = GetEntity<Transform>(MediaEntity.Transform, transformName);
            foreach (TransformOutput transformOutput in transform.Outputs)
            {
                if (transformOutput.Preset is BuiltInStandardEncoderPreset builtInEncoderPreset)
                {
                    if (builtInEncoderPreset.PresetName == EncoderNamedPreset.ContentAwareEncodingExperimental)
                    {
                        transformPresets.Add(MediaTransformPreset.ContentAwareEncoding);
                    }
                    else
                    {
                        transformPresets.Add(MediaTransformPreset.AdaptiveStreaming);
                    }
                }
                else if (transformOutput.Preset is StandardEncoderPreset standardEncoderPreset)
                {
                    if (false) // TBD
                    {
                        transformPresets.Add(MediaTransformPreset.ThumbnailSprite);
                    }
                    else
                    {
                        transformPresets.Add(MediaTransformPreset.ThumbnailImages);
                    }
                }
                else if (transformOutput.Preset is VideoAnalyzerPreset)
                {
                    transformPresets.Add(MediaTransformPreset.VideoAnalyzer);
                }
                else if (transformOutput.Preset is AudioAnalyzerPreset)
                {
                    transformPresets.Add(MediaTransformPreset.AudioAnalyzer);
                }
                else if (transformOutput.Preset is FaceDetectorPreset)
                {
                    transformPresets.Add(MediaTransformPreset.FaceDetector);
                }
            }
            return transformPresets.ToArray();
        }

        private StandardEncoderPreset GetThumbnailGeneration(Image thumbnailCodec)
        {
            StandardEncoderPreset customEncoding = new StandardEncoderPreset
            {
                Codecs = new List<Codec>(),
                Formats = new List<Format>()
            };
            JpgFormat thumbnailFormat = new JpgFormat()
            {
                FilenamePattern = string.Concat(Constant.Media.Thumbnail.FileNamePrefix, Constant.Media.Thumbnail.FileNameSuffix)
            };
            customEncoding.Codecs.Add(thumbnailCodec);
            customEncoding.Formats.Add(thumbnailFormat);
            return customEncoding;
        }

        private string GetTransformName(string transformName, MediaTransformOutput[] transformOutputs)
        {
            if (string.IsNullOrEmpty(transformName))
            {
                foreach (MediaTransformOutput transformOutput in transformOutputs)
                {
                    if (transformOutput.PresetType != MediaTransformPreset.VideoIndexer &&
                        transformOutput.PresetType != MediaTransformPreset.AudioIndexer)
                    {
                        if (!string.IsNullOrEmpty(transformName))
                        {
                            transformName = string.Concat(transformName, Constant.TextDelimiter.TransformName);
                        }
                        transformName = string.Concat(transformName, transformOutput.PresetName);
                    }
                }
            }
            return transformName;
        }

        private TransformOutput GetTransformOutput(MediaTransformOutput transformOutput, Image thumbnailCodec)
        {
            Preset transformPreset = null;
            switch (transformOutput.PresetType)
            {
                case MediaTransformPreset.AdaptiveStreaming:
                    EncoderNamedPreset presetName = EncoderNamedPreset.AdaptiveStreaming;
                    if (!string.IsNullOrEmpty(transformOutput.PresetName))
                    {
                        presetName = transformOutput.PresetName;
                    }
                    transformPreset = new BuiltInStandardEncoderPreset(presetName);
                    break;
                case MediaTransformPreset.ContentAwareEncoding:
                    transformPreset = new BuiltInStandardEncoderPreset(EncoderNamedPreset.ContentAwareEncodingExperimental);
                    break;
                case MediaTransformPreset.ThumbnailImages:
                case MediaTransformPreset.ThumbnailSprite:
                    transformPreset = GetThumbnailGeneration(thumbnailCodec);
                    break;
                case MediaTransformPreset.VideoAnalyzer:
                    transformPreset = new VideoAnalyzerPreset()
                    {
                        InsightsToExtract = InsightsType.VideoInsightsOnly
                    };
                    break;
                case MediaTransformPreset.AudioAnalyzer:
                    transformPreset = new AudioAnalyzerPreset();
                    break;
                case MediaTransformPreset.FaceDetector:
                    transformPreset = new FaceDetectorPreset();
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

        public Transform GetTransform(string transformName, string transformDescription, MediaTransformOutput[] transformOutputs, Image thumbnailCodec, bool createUpdate)
        {
            Transform transform = null;
            List<TransformOutput> transformOutputList = new List<TransformOutput>();
            foreach (MediaTransformOutput transformOutput in transformOutputs)
            {
                TransformOutput transformOutputItem = GetTransformOutput(transformOutput, thumbnailCodec);
                if (transformOutputItem != null)
                {
                    transformOutputList.Add(transformOutputItem);
                }
            }
            if (transformOutputList.Count > 0)
            {
                transformName = GetTransformName(transformName, transformOutputs);
                if (!createUpdate)
                {
                    transform = _media.Transforms.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName);
                }
                if (transform == null)
                {
                    transform = _media.Transforms.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, transformOutputList.ToArray(), transformDescription);
                }
            }
            return transform;
        }

        public Transform GetTransform(bool adaptiveStreaming, bool contentAwareEncoding, bool thumbnailImages, bool thumbnailSprite,
                                      bool videoAnalyzer, bool audioAnalyzer, bool faceDetector, bool videoIndexer, bool audioIndexer)
        {
            List<MediaTransformOutput> transformOutputs = new List<MediaTransformOutput>();
            if (adaptiveStreaming)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.AdaptiveStreaming,
                    PresetName = EncoderNamedPreset.AdaptiveStreaming.ToString()
                };
                transformOutputs.Add(transformOutput);
            }
            if (contentAwareEncoding)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.ContentAwareEncoding,
                    PresetName = EncoderNamedPreset.ContentAwareEncodingExperimental.ToString()
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
            if (thumbnailSprite)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.ThumbnailSprite,
                    PresetName = MediaTransformPreset.ThumbnailSprite.ToString()
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
            if (faceDetector)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.FaceDetector,
                    PresetName = MediaTransformPreset.FaceDetector.ToString()
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
            return GetTransform(null, null, transformOutputs.ToArray(), null, false);
        }

        public Transform GetTransform(MediaTransformPreset[] transformPresets)
        {
            bool adaptiveStreaming = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.AdaptiveStreaming);
            bool contentAwareEncoding = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.ContentAwareEncoding);
            bool thumbnailImages = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.ThumbnailImages);
            bool thumbnailSprite = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.ThumbnailSprite);
            bool videoAnalyzer = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.VideoAnalyzer);
            bool audioAnalyzer = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.AudioAnalyzer);
            bool faceDetector = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.FaceDetector);
            bool videoIndexer = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.VideoIndexer);
            bool audioIndexer = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.AudioIndexer);
            return GetTransform(adaptiveStreaming, contentAwareEncoding, thumbnailImages, thumbnailSprite, videoAnalyzer, audioAnalyzer, faceDetector, videoIndexer, audioIndexer);
        }
    }
}