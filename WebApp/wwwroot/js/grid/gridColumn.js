var defaultWidth = 120, typeWidth = 150, typeWidthEx = 200, nameWidth = 300, nameWidthEx = 400;
function GetParentColumns(gridId) {
    var columns;
    switch (gridId) {
        case "storageAccounts":
            columns = [
                {
                    formatter: FormatName,
                    label: "Storage Account Name",
                    name: "id",
                    align: "center",
                    width: nameWidth
                },
                {
                    label: "Storage Type",
                    name: "accountType",
                    align: "center",
                    width: defaultWidth
                },
                {
                    label: "Access Tier",
                    name: "accessTier",
                    align: "center",
                    width: defaultWidth
                },
                {
                    label: "Media Type",
                    name: "type",
                    align: "center",
                    width: defaultWidth
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
                    label: "Storage",
                    name: "properties.storageAccountName",
                    align: "center",
                    width: typeWidthEx
                },
                {
                    label: "Size",
                    name: "size",
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
                    width: nameWidth
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
                    width: 400
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
                    width: 400
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
                    width: 400
                },
                {
                    label: "Streaming Policy Name",
                    name: "properties.streamingPolicyName",
                    align: "center",
                    width: nameWidth
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
                    width: typeWidthEx
                },
                {
                    formatter: FormatLiveEncodingType,
                    label: "Encoding Type",
                    name: "properties.encoding",
                    align: "center",
                    width: typeWidthEx
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
                    label: "Asset Name",
                    name: "properties.assetName",
                    align: "center",
                    width: nameWidth
                },
                {
                    formatter: FormatName,
                    label: "Manifest Name",
                    name: "properties.manifestName",
                    align: "center",
                    width: nameWidth
                },
                {
                    label: "Archive Window",
                    name: "properties.archiveWindowLength",
                    align: "center",
                    width: typeWidthEx
                }
            ];
            break;
        case "indexerInsights":
            columns = [
                {
                    label: "Index Id",
                    name: "id",
                    align: "center",
                    width: defaultWidth
                },
                {
                    formatter: FormatName,
                    label: "Video Name",
                    name: "name",
                    align: "center",
                    width: nameWidthEx
                }
            ];
            break;
    }
    if (gridId != "storageAccounts" &&
        gridId != "indexerInsights") {
        var created = {
            formatter: FormatDateTime,
            label: "Created",
            name: "properties.created",
            align: "center",
            width: defaultWidth
        };
        columns.splice(columns.length, 0, created);
    }
    if (gridId != "storageAccounts" &&
        gridId != "streamingPolicies" &&
        gridId != "streamingLocators" &&
        gridId != "indexerInsights") {
        var modified = {
            formatter: FormatDateTime,
            label: "Modified",
            name: "properties.lastModified",
            align: "center",
            width: defaultWidth
        };
        columns.splice(columns.length, 0, modified);
    }
    if (gridId != "storageAccounts") {
        var actions = {
            formatter: FormatActions,
            label: "Actions",
            align: "center",
            width: defaultWidth,
            sortable: false
        };
        columns.splice(columns.length, 0, actions);
    }
    return columns;
}
function GetChildColumns(gridId) {
    var columns;
    switch (gridId) {
        case "assetFiles":
            columns = [
                {
                    formatter: FormatName,
                    label: "Media Asset File Name",
                    name: "name",
                    align: "center",
                    width: nameWidthEx + 330
                },
                {
                    label: "Size",
                    name: "size",
                    align: "center",
                    width: defaultWidth,
                    sortable: false
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
                    name: "preset.presetName",
                    align: "center",
                    width: nameWidthEx
                },
                {
                    label: "Priority",
                    name: "relativePriority",
                    align: "center",
                    width: defaultWidth + 5
                },
                {
                    formatter: FormatValue,
                    label: "On Error",
                    name: "onError",
                    align: "center",
                    width: defaultWidth + 5
                }
            ];
            break;
        case "transformJobOutputs":
            columns = [
                {
                    formatter: FormatName,
                    label: "Job Output Asset Name",
                    name: "assetName",
                    align: "center",
                    width: nameWidthEx + 125
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
                    width: defaultWidth + 5
                }
            ];
            break;
    }
    return columns;
}
function FormatName(value, grid, row) {
    if (row.preset != null) {
        if (row.preset.audioLanguage != null) {
            value = row.preset.audioInsightsOnly == null ? "Audio Analyzer" : "Video Analyzer";
        } else if (row.preset.codecs != null) {
            value = "Thumbnail Sprite";
        }
    }
    value = FormatValue(value, grid, row);
    var description = row["properties.description"];
    if (description != null) {
        value = "<span class=\"siteLink\" onclick=DisplayMessage(\"Description\",\"" + encodeURIComponent(description) + "\")>" + value + "</span>";
    }
    value = value.replace("Microsoft.Media/mediaservices", "");
    value = value.replace("Microsoft.Media", "");
    return value;
}
function FormatValue(value, grid, row) {
    if (value == "StopProcessingJob") {
        value = "StopJob";
    }
    for (var i = 0; i < _spacingPatterns.length; i++) {
        var expression = new RegExp(_spacingPatterns[i], "g");
        value = value.replace(expression, _spacingInserts[i]);
    }
    return value;
}
function FormatRegions(value, grid, row) {
    return "Primary: " + value + "<br>Secondary: " + row.secondaryRegion;
}
function FormatProgress(value, grid, row) {
    return value + "%";
}
function FormatDateTime(value, grid, row) {
    if (value == null) {
        value = "N/A";
    } else {
        value = value.slice(11, 19) + "<br>" + value.slice(0, 10);
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
        value = "Empty";
    } else {
        var title = "Media Job Correlation Data";
        var jsonData = JSON.stringify(value);
        value = "<span class=\"siteLink\" onclick=DisplayJson(\"" + encodeURIComponent(title) + "\",\"" + encodeURIComponent(jsonData) + "\")>Job Data</span>";
    }
    return value;
}
function FormatLiveEncodingType(value, grid, row) {
    value = value["encodingType"];
    return FormatValue(value);
}
function FormatLiveInputProtocol(value, grid, row) {
    value = value["streamingProtocol"];
    return FormatValue(value);
}