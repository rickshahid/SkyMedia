var defaultWidth = 120, typeWidth = 150, typeWidthEx = 200, nameWidth = 300, nameWidthEx = 400;
function GetParentColumns(gridId) {
    var columns;
    switch (gridId) {
        case "storageAccounts":
            columns = [
                {
                    formatter: FormatName,
                    label: "Storage Account Name",
                    name: "name",
                    align: "center",
                    width: nameWidth
                },
                {
                    formatter: FormatStorage,
                    label: "Storage Type",
                    name: "accountType",
                    align: "center",
                    width: typeWidth
                },
                {
                    label: "Media Type",
                    name: "type",
                    align: "center",
                    width: typeWidth
                },
                {
                    label: "HTTPS Only",
                    name: "httpsOnly",
                    align: "center",
                    width: defaultWidth
                },
                {
                    label: "Encryption",
                    name: "encryption",
                    align: "center",
                    width: defaultWidth
                },
                {
                    label: "Replication",
                    name: "replication",
                    align: "center",
                    width: typeWidth
                },
                {
                    formatter: FormatRegions,
                    label: "Regions",
                    name: "primaryRegion",
                    align: "center",
                    width: typeWidthEx
                }
            ];
            break;
        case "assets":
            columns = [
                {
                    formatter: FormatName,
                    label: "Media Asset Name",
                    name: "name",
                    align: "center",
                    width: nameWidthEx
                },
                {
                    label: "Alternate Id",
                    name: "properties.alternateId",
                    align: "center",
                    width: defaultWidth
                },
                {
                    formatter: FormatAssetSize,
                    label: "Size",
                    name: "size",
                    align: "center",
                    width: defaultWidth
                },
                {
                    formatter: FormatAssetLocators,
                    label: "Stream Urls",
                    name: "streamingUrls",
                    align: "center",
                    width: defaultWidth
                }
            ];
            break;
        case "transforms":
            columns = [
                {
                    formatter: FormatName,
                    label: "Media Transform Name",
                    name: "name",
                    align: "center",
                    width: nameWidthEx
                }
            ];
            break;
        case "transformJobs":
            columns = [
                {
                    formatter: FormatName,
                    label: "Media Job Name",
                    name: "name",
                    align: "center",
                    width: nameWidthEx
                },
                {
                    label: "Priority",
                    name: "properties.priority",
                    align: "center",
                    width: defaultWidth
                },
                {
                    label: "State",
                    name: "properties.state",
                    align: "center",
                    width: defaultWidth
                },
                {
                    formatter: FormatJobData,
                    label: "Data",
                    name: "properties.correlationData",
                    align: "center",
                    width: defaultWidth
                }
            ];
            break;
        case "contentKeyPolicies":
            columns = [
                {
                    formatter: FormatName,
                    label: "Content Key Policy Name",
                    name: "name",
                    align: "center",
                    width: nameWidthEx
                }
            ];
            break;
        case "streamingPolicies":
            columns = [
                {
                    formatter: FormatName,
                    label: "Streaming Policy Name",
                    name: "name",
                    align: "center",
                    width: nameWidthEx
                }
            ];
            break;
        case "streamingEndpoints":
            columns = [
                {
                    formatter: FormatName,
                    label: "Streaming Endpoint Name",
                    name: "name",
                    align: "center",
                    width: nameWidth
                }
            ];
            break;
        case "streamingLocators":
            columns = [
                {
                    formatter: FormatName,
                    label: "Streaming Locator Name",
                    name: "name",
                    align: "center",
                    width: nameWidthEx
                },
                {
                    label: "Streaming Policy Name",
                    name: "properties.streamingPolicyName",
                    align: "center",
                    width: nameWidthEx
                },
                {
                    formatter: FormatDateTime,
                    label: "Start Time",
                    name: "properties.startTime",
                    align: "center",
                    width: defaultWidth
                },
                {
                    formatter: FormatDateTime,
                    label: "End Time",
                    name: "properties.endTime",
                    align: "center",
                    width: defaultWidth
                }
            ];
            break;
        case "streamingFiltersAccount":
            columns = [
                {
                    formatter: FormatName,
                    label: "Account Filter Name",
                    name: "name",
                    align: "center",
                    width: nameWidth
                },
                {
                    formatter: FormatQuality,
                    label: "1st Quality (bps)",
                    name: "properties.firstQuality",
                    align: "center",
                    width: typeWidth
                },
                {
                    formatter: FormatTrackSelection,
                    label: "Track Selection",
                    name: "properties.tracks",
                    align: "center",
                    width: typeWidth
                },
                {
                    formatter: FormatTimeRange,
                    label: "Presentation Range",
                    name: "properties.presentationTimeRange",
                    align: "center",
                    width: typeWidthEx
                }
            ];
            break;
        case "streamingFiltersAsset":
            columns = [
                {
                    formatter: FormatName,
                    label: "Asset Filter Name",
                    name: "name",
                    align: "center",
                    width: nameWidth
                },
                {
                    formatter: FormatQuality,
                    label: "1st Quality (bps)",
                    name: "properties.firstQuality",
                    align: "center",
                    width: typeWidth
                },
                {
                    formatter: FormatTrackSelection,
                    label: "Track Selection",
                    name: "properties.tracks",
                    align: "center",
                    width: typeWidth
                },
                {
                    formatter: FormatTimeRange,
                    label: "Presentation Range",
                    name: "properties.presentationTimeRange",
                    align: "center",
                    width: typeWidthEx
                }
            ];
            break;
        case "liveEvents":
            columns = [
                {
                    formatter: FormatName,
                    label: "Live Event Name",
                    name: "name",
                    align: "center",
                    width: nameWidth
                },
                {
                    formatter: FormatLiveInputProtocol,
                    label: "Input Protocol",
                    name: "properties.input",
                    align: "center",
                    width: typeWidth
                },
                {
                    formatter: FormatLiveEncoding,
                    label: "Encoding",
                    name: "properties.encoding",
                    align: "center",
                    width: defaultWidth
                },
                {
                    label: "State",
                    name: "properties.resourceState",
                    align: "center",
                    width: defaultWidth
                }
            ];
            break;
        case "liveEventOutputs":
            columns = [
                {
                    formatter: FormatName,
                    label: "Live Event Output Name",
                    name: "name",
                    align: "center",
                    width: nameWidth
                },
                {
                    Formatter: FormatName,
                    label: "Output Asset Name",
                    name: "properties.assetName",
                    align: "center",
                    width: nameWidth
                },
                {
                    label: "DVR Window",
                    name: "properties.archiveWindowLength",
                    align: "center",
                    width: typeWidth
                }
            ];
            break;
        case "indexerInsights":
            columns = [
                {
                    label: "Insight Id",
                    name: "id",
                    align: "center",
                    width: defaultWidth
                },
                {
                    formatter: FormatName,
                    label: "Video Name",
                    name: "name",
                    align: "center",
                    width: nameWidth
                },
                {
                    label: "State",
                    name: "state",
                    align: "center",
                    width: defaultWidth
                },
                {
                    formatter: FormatProgress,
                    label: "Progress",
                    name: "processingProgress",
                    align: "center",
                    width: defaultWidth
                }
            ];
            break;
    }
    if (gridId != "storageAccounts" &&
        gridId != "streamingFiltersAccount" &&
        gridId != "streamingFiltersAsset" &&
        gridId != "indexerInsights") {
        var created = {
            formatter: FormatDateTime,
            label: "Created",
            name: "properties.created",
            align: "center",
            width: defaultWidth
        };
        columns.push(created);
    }
    if (gridId != "storageAccounts" &&
        gridId != "streamingPolicies" &&
        gridId != "streamingLocators" &&
        gridId != "streamingFiltersAccount" &&
        gridId != "streamingFiltersAsset" &&
        gridId != "indexerInsights") {
        var modified = {
            formatter: FormatDateTime,
            label: "Modified",
            name: "properties.lastModified",
            align: "center",
            width: defaultWidth
        };
        columns.push(modified);
    }
    if (gridId != "storageAccounts") {
        var width = defaultWidth;
        if (gridId == "liveEvents") {
            width = typeWidth;
        }
        var actions = {
            formatter: FormatActions,
            label: "Actions",
            name: "actions",
            align: "center",
            width: width,
            sortable: false
        };
        columns.push(actions);
    }
    var parentColumn = {
        name: "parentName",
        hidden: true
    };
    columns.unshift(parentColumn);
    return columns;
}
function GetChildColumns(gridType) {
    var columns;
    switch (gridType) {
        case "assetFiles":
            columns = [
                {
                    formatter: FormatName,
                    label: "Media Asset File Name",
                    name: "name",
                    align: "center",
                    width: nameWidthEx + 125
                },
                {
                    label: "Size",
                    name: "size",
                    align: "center",
                    width: defaultWidth
                },
                {
                    label: "Content Type",
                    name: "contentType",
                    align: "center",
                    width: typeWidthEx + 50
                },
                {
                    formatter: FormatActions,
                    label: "Actions",
                    align: "center",
                    width: defaultWidth,
                    sortable: false
                }
            ];
            break;
        case "transformOutputs":
            columns = [
                {
                    formatter: FormatName,
                    label: "Transform Output Preset Name",
                    name: "preset",
                    align: "center",
                    width: nameWidthEx
                },
                {
                    label: "Priority",
                    name: "relativePriority",
                    align: "center",
                    width: defaultWidth
                },
                {
                    formatter: FormatValue,
                    label: "On Error",
                    name: "onError",
                    align: "center",
                    width: defaultWidth
                }
            ];
            break;
        case "transformJobOutputs":
            columns = [
                {
                    formatter: FormatName,
                    label: "Media Job Output Label",
                    name: "label",
                    align: "center",
                    width: nameWidthEx + 120
                },
                {
                    formatter: FormatJobOutputState,
                    label: "State",
                    name: "state",
                    align: "center",
                    width: defaultWidth
                },
                {
                    formatter: FormatProgress,
                    label: "Progress",
                    name: "progress",
                    align: "center",
                    width: defaultWidth
                },
                {
                    formatter: FormatName,
                    label: "Output Asset Name",
                    name: "assetName",
                    align: "center",
                    width: nameWidth + 60
                }
            ];
            break;
        case "contentKeyPolicyOptions":
            columns = [
                {
                    label: "Content Key Policy Option Id",
                    name: "policyOptionId",
                    align: "center",
                    width: nameWidthEx - 5
                },
                {
                    label: "Content Key Policy Option Name",
                    name: "name",
                    align: "center",
                    width: nameWidth + 60
                }
            ];
            break;
        case "liveEventOutputs":
            columns = [
                {
                    formatter: FormatName,
                    label: "Live Event Output Name",
                    name: "name",
                    align: "center",
                    width: nameWidth
                },
                {
                    Formatter: FormatName,
                    label: "Output Asset Name",
                    name: "properties.assetName",
                    align: "center",
                    width: nameWidth
                },
                {
                    label: "DVR Window",
                    name: "properties.archiveWindowLength",
                    align: "center",
                    width: typeWidth
                },
                {
                    formatter: FormatName,
                    label: "Manifest Name",
                    name: "properties.manifestName",
                    align: "center",
                    width: nameWidth + 30
                }
            ];
           break;
    }
    return columns;
}
function FormatName(value, grid, row) {
    if (row.preset != null) {
        if (row.preset.presetName != null) {
            value = row.preset.presetName;
        } else if (row.preset.hasOwnProperty("audioLanguage")) {
            value = row.preset.insightsToExtract != null ? "Video Analyzer" : "Audio Analyzer";
        } else if (row.preset.hasOwnProperty("codecs")) {
            value = "Thumbnail Images";
        } else {
            value = "Face Detector";
        }
    }
    value = FormatValue(value);
    var title = "", message = "";
    var container = row["properties.container"];
    var description = row["properties.description"];
    var streamOptions = row["properties.streamOptions"];
    if (container != null && container != "") {
        title = "Container";
        message = container;
    }
    if (description != null && description != "") {
        if (title != "") {
            title = title + " & ";
        }
        title = title + "Description";
        if (message != "") {
            message = message + "<br><br>";
        }
        message = message + description;
    }
    if (streamOptions != null && streamOptions != "") {
        if (title != "") {
            title = title + " & ";
        }
        title = title + "Stream Options";
        if (message != "") {
            message = message + "<br><br>";
        }
        var options = streamOptions.join(", ");
        message = message + FormatValue(options);
    }
    if (message != "") {
        value = "<span class=\"siteLink\" onclick=DisplayMessage(\"" + encodeURIComponent(title) + "\",\"" + encodeURIComponent(message) + "\")>" + value + "</span>";
    }
    //value = value.replace("Microsoft.Media/mediaservices", "");
    //value = value.replace("Microsoft.Media", "");
    return value;
}
function FormatStorage(value, grid, row) {
    if (value.length <= 4) {
        value = value + " ";
    } else {
        value = value + "<br>";
    }
    return value + "(" + row.accessTier + ")";
}
function FormatRegions(value, grid, row) {
    return value + " (1st)<br>" + row.secondaryRegion + " (2nd)";
}
function FormatProgress(value, grid, row) {
    if (value == null) {
        value = "0";
    }
    if (value.toString().indexOf("%") == -1) {
        value = value + "%";
    }
    return value;
}
function FormatQuality(value, grid, row) {
    return value == null ? "" : value.bitrate;
}
function FormatTrackSelection(value, grid, row) {
    return value == null ? 0 : value.length;
}
function FormatTimeRange(value, grid, row) {
    return value == null ? "" : "";
}
function FormatDateTime(value, grid, row) {
    if (value == null) {
        value = "N/A";
    } else {
        value = value.slice(11, 19) + "<br>" + value.slice(0, 10);
    }
    return value;
}
function FormatAssetSize(value, grid, row) {
    var storageAccountName = row["properties.storageAccountName"];
    return "<span class=\"siteLink\" onclick=DisplayMessage(\"Storage%20Account%20Name\",\"" + storageAccountName + "\")>" + value + "</span>";
}
function FormatAssetLocators(value, grid, row) {
    var urlCount = value.length + " Url";
    if (value.length != 1) {
        urlCount = urlCount + "s";
    }
    var streamingUrls = "";
    for (var i = 0; i < value.length; i++) {
        if (streamingUrls != "") {
            streamingUrls = streamingUrls + "<br><br>";
        }
        streamingUrls = streamingUrls + value[i];
    }
    if (value.length == 0) {
        value = urlCount;
    } else {
        value = "<span class=\"siteLink\" onclick=DisplayMessage(\"Media%20Asset%20Streaming%20Locators\",\"" + encodeURIComponent(streamingUrls) + "\")>" + urlCount + "</span>";
    }
    return value;
}
function FormatJobOutputState(value, grid, row) {
    if (value == "Error") {
        var title = row.error.category + " Error";
        var message = FormatValue(row.error.code);
        if (row.error.details.length > 0) {
            title = FormatValue(row.error.details[0].code + " (" + row.error.retry + ")");
            message = row.error.details[0].message;
        }
        value = "<span class=\"siteLink\" onclick=DisplayMessage(\"" + encodeURIComponent(title) + "\",\"" + encodeURIComponent(message) + "\")>" + value + "</span>";
    }
    return value;
}
function FormatJobData(value, grid, row) {
    if (jQuery.isEmptyObject(value)) {
        value = "0 Items";
    } else {
        var title = "Media Job Data";
        var jsonData = JSON.stringify(value);
        var itemCount = Object.keys(value).length;
        value = "<span class=\"siteLink\" onclick=DisplayJson(\"" + encodeURIComponent(title) + "\",\"" + encodeURIComponent(jsonData) + "\")>" + itemCount + " Items</span>";
    }
    return value;
}
function AddEndpointUrls(endpoints, endpointUrls) {
    for (var i = 0; i < endpoints.length; i++) {
        var endpoint = endpoints[i];
        if (endpointUrls != "") {
            endpointUrls = endpointUrls + "<br><br>";
        }
        endpointUrls = endpointUrls + endpoint["url"];
    }
    return endpointUrls;
}
function FormatLiveInputProtocol(value, grid, row) {
    var endpointUrls = "";
    var endpoints = value["endpoints"];
    endpointUrls = AddEndpointUrls(endpoints, endpointUrls);
    endpoints = row["properties.preview"]["endpoints"];
    endpointUrls = AddEndpointUrls(endpoints, endpointUrls);
    var inputProtocol = FormatValue(value["streamingProtocol"]);
    return "<span class=\"siteLink\" onclick=DisplayMessage(\"Live%20Input%20&%20Preview%20Endpoints\",\"" + encodeURIComponent(endpointUrls) + "\")>" + inputProtocol + "</span>";
}
function FormatLiveEncoding(value, grid, row) {
    var encodingType = value["encodingType"];
    if (encodingType == "None") {
        value = encodingType;
    } else {
        var encodingPresetName = value["presetName"];
        value = "<span class=\"siteLink\" onclick=DisplayMessage(\"Live%20Encoding%20Preset%20Name\",\"" + encodeURIComponent(encodingPresetName) + "\")>" + encodingType + "</span>";
    }
    return value;
}