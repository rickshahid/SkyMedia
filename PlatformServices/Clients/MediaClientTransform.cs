using System.Linq;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
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
                    if (!string.IsNullOrEmpty(transformName))
                    {
                        transformName = string.Concat(transformName, Constant.Media.Transform.Preset.NameDelimiter);
                    }
                    transformName = string.Concat(transformName, transformOutput.PresetName);
                }
            }
            return transformName;
        }

        private TransformOutput GetTransformOutput(MediaTransformOutput transformOutput, Image thumbnailCodec)
        {
            Preset transformPreset = null;
            switch (transformOutput.PresetType)
            {
                case MediaTransformPreset.ContentAwareEncoding:
                    transformPreset = new BuiltInStandardEncoderPreset(EncoderNamedPreset.ContentAwareEncodingExperimental);
                    break;
                case MediaTransformPreset.AdaptiveStreaming:
                    EncoderNamedPreset presetName = EncoderNamedPreset.AdaptiveStreaming;
                    if (!string.IsNullOrEmpty(transformOutput.PresetName))
                    {
                        presetName = transformOutput.PresetName;
                    }
                    transformPreset = new BuiltInStandardEncoderPreset(presetName);
                    break;
                case MediaTransformPreset.ThumbnailImages:
                case MediaTransformPreset.ThumbnailSprite:
                    transformPreset = GetThumbnailGeneration(thumbnailCodec);
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

        public Transform GetTransform(bool contentAwareEncoding, bool adaptiveStreaming, bool thumbnailImages, bool thumbnailSprite, bool videoAnalyzer, bool audioAnalyzer, bool videoIndexer, bool audioIndexer)
        {
            List<MediaTransformOutput> transformOutputs = new List<MediaTransformOutput>();
            if (contentAwareEncoding)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.ContentAwareEncoding,
                    PresetName = EncoderNamedPreset.ContentAwareEncodingExperimental.ToString()
                };
                transformOutputs.Add(transformOutput);
            }
            if (adaptiveStreaming)
            {
                MediaTransformOutput transformOutput = new MediaTransformOutput()
                {
                    PresetType = MediaTransformPreset.AdaptiveStreaming,
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
            bool contentAwareEncoding = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.ContentAwareEncoding);
            bool adaptiveStreaming = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.AdaptiveStreaming);
            bool thumbnailImages = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.ThumbnailImages);
            bool thumbnailSprite = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.ThumbnailSprite);
            bool videoAnalyzer = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.VideoAnalyzer);
            bool audioAnalyzer = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.AudioAnalyzer);
            bool videoIndexer = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.VideoIndexer);
            bool audioIndexer = transformPresets.Contains<MediaTransformPreset>(MediaTransformPreset.AudioIndexer);
            return GetTransform(contentAwareEncoding, adaptiveStreaming, thumbnailImages, thumbnailSprite, videoAnalyzer, audioAnalyzer, videoIndexer, audioIndexer);
        }
    }
}