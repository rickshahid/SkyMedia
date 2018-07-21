using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class TransformController : Controller
    {
        private static Preset GetAnalyzerPreset(bool audioOnly)
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

        private static TransformOutput GetTransformOutput(Preset transformPreset, MediaTransformOutput transformOutput)
        {
            return new TransformOutput(transformPreset)
            {
                RelativePriority = (Priority)transformOutput.RelativePriority,
                OnError = (OnErrorType)transformOutput.OnError
            };
        }

        internal static Transform CreateTransform(MediaClient mediaClient, bool standardEncoderPreset, bool videoAnalyzerPreset, bool audioAnalyzerPreset)
        {
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
            return CreateTransform(mediaClient, null, null, transformOutputs);
        }

        internal static Transform CreateTransform(MediaClient mediaClient, string transformName, string transformDescription, MediaTransformOutput[] transformOutputs)
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
                transform = mediaClient.CreateTransform(transformName, transformDescription, transformOutputList.ToArray());
            }
            return transform;
        }

        public JsonResult Create(string transformName, string transformDescription, MediaTransformOutput[] transformOutputs)
        {
            Transform transform;
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                transform = CreateTransform(mediaClient, transformName, transformDescription, transformOutputs);
            }
            return Json(transform);
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transforms"] = mediaClient.GetAllEntities<Transform>(MediaEntity.Transform);
            }
            return View();
        }
    }
}