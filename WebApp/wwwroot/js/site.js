function SignOut(cookieName) {
    $.removeCookie(cookieName);
    window.location.href = "/account/signout";
}
function ReplaceAll(text, find, replace) {
    var regExp = new RegExp(find, "g");
    return text.replace(regExp, replace);
}
function CreateTip(targetId, tipText, tipPosition) {
    $("#" + targetId).qtip({
        content: { text: tipText },
        position: tipPosition,
        show: { delay: 2000 }
    });
}
function CreateTipTop(targetId, tipText) {
    var tipPosition = { my: "bottom center", at: "top center" };
    CreateTip(targetId, tipText, tipPosition);
}
function CreateTipTopLeft(targetId, tipText, adjustX, adjustY) {
    var tipPosition = { my: "bottom center", at: "top left", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition);
}
function CreateTipBottom(targetId, tipText) {
    var tipPosition = { my: "top center", at: "bottom center" };
    CreateTip(targetId, tipText, tipPosition);
}
function CreateTipBottomLeft(targetId, tipText, adjustX, adjustY) {
    var tipPosition = { my: "top center", at: "bottom left", adjust: { x: adjustX, y: adjustY } };
    CreateTip(targetId, tipText, tipPosition);
}
function CreateTipLeft(targetId, tipText) {
    var tipPosition = { my: "right center", at: "left center" };
    CreateTip(targetId, tipText, tipPosition);
}
function CreateTipRight(targetId, tipText) {
    var tipPosition = { my: "left center", at: "right center" };
    CreateTip(targetId, tipText, tipPosition);
}
function SetTipVisible(targetId, tipVisible) {
    $("#" + targetId).qtip("toggle", tipVisible);
}
function SetLayout() {
    CreateTipRight("siteHome", "Azure Sky Media<br /><br />Site Home");
    CreateTipRight("siteCode", "Azure Sky Media<br /><br />Open Source");
    CreateTipRight("accountDashboard", "Azure Media Services<br /><br />Account Dashboard");
    CreateTipRight("accountContext", "Azure Media Services<br /><br />Account Name");
    CreateTipLeft("userProfileEdit", "Azure Active Directory<br /><br />User Profile Edit");
    CreateTipLeft("userSignIn", "Azure Active Directory<br /><br />User Sign Up & In");
    CreateTipLeft("userSignOut", "Azure Active Directory<br /><br />User Sign Out");
    CreateTipRight("amsPlatform", "Azure Media Services");
    CreateTipRight("amsPlayer", "Azure Media Player");
    CreateTipRight("channelIngest", "Azure Media Services<br /><br />Live Channel");
    CreateTipRight("mediaAnalytics", "Azure Media Services<br /><br />Media Analytics");
    CreateTipRight("mediaEditor", "Azure Media Services<br /><br />Video Editor");
    CreateTipRight("contentNetwork", "Azure CDN<br /><br />(Content Delivery Network)");
    CreateTipLeft("appServiceWeb", "Azure App Service<br /><br />Web Apps");
    CreateTipLeft("appServiceMobile", "Azure App Service<br /><br />Mobile Apps");
    CreateTipLeft("appServiceApi", "Azure App Service<br /><br />API Apps");
    CreateTipLeft("appServiceApiManagement", "Azure App Service<br /><br />API Management");
    CreateTipLeft("appServiceLogic", "Azure App Service<br /><br />Logic Apps");
    CreateTipLeft("appServiceFunctions", "Azure App Service<br /><br />Functions");
    CreateTipTop("mediaFileUpload", "Azure Media Services<br /><br />File Uploader");
    CreateTipTop("mediaAssetWorkflow", "Azure Media Services<br /><br />Asset Workflow");
    CreateTipTop("mediaAssetLibrary", "Azure Media Services<br /><br />Asset Library");
    CreateTipTop("mediaAssetAnalytics", "Azure Media Services<br /><br />Media Analytics")
    $(document).ajaxError(function (event, xhr, settings, error) {
        DisplayMessage("Error Message", error);
    });
}
function DisplayDialog(dialogId, title, html, buttons, height, width, onClose) {
    if (buttons == null) {
        buttons = {
            OK: function () {
                $(this).dialog("close");
            }
        };
    }
    if (html != null) {
        html = unescape(html);
        $("#" + dialogId).html(html);
    }
    if (height == null) {
        height = "auto";
    }
    if (width == null) {
        width = "auto";
    }
    $("#" + dialogId).dialog({
        buttons: buttons,
        height: height,
        width: width,
        title: title,
        close: onClose,
        resizable: false,
        modal: true
    });
    if (jQuery.isEmptyObject(buttons)) {
        $(".ui-dialog-titlebar-close").show();
    }
    $(".ui-button:last").focus();
}
function DisplayMessage(title, message, buttons, width, onClose) {
    var dialogId = "messageDialog";
    DisplayDialog(dialogId, title, message, buttons, null, width, onClose);
}
function GetMediaPlayer(editVideo) {
    var mediaPlayer = amp("mediaPlayer");
    if (editVideo) {
        mediaPlayer.AMVE({
            containerId: "videoEditor",
            clipdataCallback: CreateSubclip
        });
    }
    $(".amp-logo").click(function () {
        window.open("http://azure.microsoft.com/en-us/services/media-services/");
    });
    return mediaPlayer;
}
function SetPlayerSpinner(visible) {
    if (visible) {
        $(".vjs-loading-spinner").show();
    } else {
        $(".vjs-loading-spinner").hide();
    }
}
function GetSourceType(sourceUrl) {
    return (sourceUrl.toLowerCase().indexOf(".mp4") > -1) ? "video/mp4" : "application/vnd.ms-sstr+xml";
}
function GetLanguageLabel(languageCode) {
    var languageLabel;
    switch (languageCode.substr(0, 2).toLowerCase()) {
        case "en":
            languageLabel = "English";
            break;
        case "es":
            languageLabel = "Spanish";
            break;
        case "ar":
            languageLabel = "Arabic";
            break;
        case "zh":
            languageLabel = "Chinese";
            break;
        case "fr":
            languageLabel = "French";
            break;
        case "de":
            languageLabel = "German";
            break;
        case "it":
            languageLabel = "Italian";
            break;
        case "ja":
            languageLabel = "Japanese";
            break;
        case "pt":
            languageLabel = "Portuguese";
            break;
    }
    return languageLabel;
}
function GetProtectionInfo(protectionTypes, authToken) {
    var protectionInfo = null;
    if (protectionTypes.length > 0) {
        protectionInfo = new Array();
        var authToken = (window.location.href.indexOf("notoken") > -1) ? "" : "Bearer=" + authToken;
        for (var i = 0; i < protectionTypes.length; i++) {
            protectionInfo.push({
                type: protectionTypes[i],
                authenticationToken: authToken
            });
        }
    }
    return protectionInfo;
}
function LoadTreeAssets(authToken, cdnUrl, libraryView) {
    var plugins = libraryView ? ["contextmenu"] : ["checkbox"];
    $("#mediaAssets").jstree({
        "core": {
            "themes": {
                "variant": "large"
            },
            "data": {
                "url": function (node) {
                    return (node.id == "#") ? "/asset/roots" : "/asset/children?assetId=" + node.id;
                },
                "data": function (node) {
                    return { "id": node.id };
                }
            }
        },
        "plugins": plugins,
        "checkbox": {
            "keep_selected_style": false,
            "tie_selection": false,
            "three_state": false,
            "whole_node": false
        },
        "contextmenu": {
            "items": function (treeNode) {
                var menuItems = {};
                if (treeNode.a_attr.isStreamable) {
                    menuItems = {
                        "Edit": {
                            "label": "Create Subclip",
                            "icon": cdnUrl + "/MediaAssetEdit.png",
                            "action": function (treeNode) {
                                DisplayEditor(treeNode, authToken);
                            }
                        }
                    };
                }
                return menuItems;
            }
        }
    });
}
function ToggleMediaAnalytics(button) {
    var buttonImage = button.children[0];
    if (buttonImage.src.indexOf("MediaAnalyticsOpen") > -1) {
        buttonImage.src = buttonImage.src.replace("Open", "Close");
        $("#analyticsPanel").show();
    } else {
        buttonImage.src = buttonImage.src.replace("Close", "Open");
        $("#analyticsPanel").hide();
    }
}