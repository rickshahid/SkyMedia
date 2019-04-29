var _jsonEditor;
function CreateJsonEditor(containerId, jsonName, jsonData) {
    var container = document.getElementById(containerId);
    var options = {
        "mode": jsonData != null ? "view" : "tree",
        "search": jsonData != null
    };
    if (_jsonEditor != null) {
        _jsonEditor.destroy();
    }
    _jsonEditor = new JSONEditor(container, options);
    if (jsonName != null) {
        _jsonEditor.setName(jsonName);
    }
    if (jsonData != null) {
        if (typeof jsonData == "string") {
            jsonData = decodeURIComponent(jsonData);
            jsonData = JSON.parse(jsonData);
        }
        for (var dataItem in jsonData) {
            if (typeof jsonData[dataItem] == "string" && jsonData[dataItem].indexOf("{") > -1 && jsonData[dataItem].indexOf("}") > -1) {
                jsonData[dataItem] = JSON.parse(jsonData[dataItem]);
            }
        }
        _jsonEditor.set(jsonData);
    }
}
//function LoadClipper(loadMode, autoPlay) {
//    var restVersion = "2.0";
//    var clipperSelector = "#videoClipper";
//    var onSubmitClip = function (clipData) {
//        var promise = new Promise(function (resolve, reject) {
//            SetCursor(true);
//            $.post("/edit/clip", {
//                clipData: clipData
//            })
//            .then(function (clip) {
//                resolve(true);
//            }).catch(function () {
//                reject();
//            });
//            SetCursor(false);
//        });
//        return promise;
//    };
//    switch (loadMode) {
//        case "static":
//            var staticClipper = new subclipper({
//                height: 600,
//                autoplayVideo: autoPlay,
//                restVersion: restVersion,
//                selector: clipperSelector,
//                submitSubclipCallback: onSubmitClip
//            });
//            SetCursor(true);
//            $.get("/edit/streams",
//                {
//                    assetName: assetName
//                },
//                function (streams) {
//                    SetCursor(false);
//                    clipper.load(streams);
//                }
//            );
//            break;
//        case "dynamic":
//            var onLoadPanel = function (assetName, skipCount, takeCount, streamType) {
//                var promise = new Promise(function (resolve, reject) {
//                    SetCursor(true);
//                    $.get("/edit/streams", {
//                        assetName: assetName,
//                        skipCount: skipCount,
//                        takeCount: takeCount,
//                        streamType: streamType
//                    })
//                    .then(function (streams) {
//                        resolve({
//                            total: streams.length,
//                            assets: streams
//                        });
//                    }).catch(function () {
//                        reject();
//                    });
//                    SetCursor(false);
//                });
//                return promise;
//            };
//            var dynamicClipper = new subclipper({
//                height: 850,
//                autoplayVideo: autoPlay,
//                restVersion: restVersion,
//                selector: clipperSelector,
//                submitSubclipCallback: onSubmitClip,
//                assetsPanelLoaderCallback: onLoadPanel
//            });
//            break;
//    }
//}