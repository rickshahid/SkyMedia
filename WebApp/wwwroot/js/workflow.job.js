function ScrollToBottom() {
    $.scrollTo("#mediaWorkflowTaskAdd");
    $.scrollTo("#mediaWorkflowTaskRemove");
}
function GetJob() {
    var jobTasks = GetJobTasks();
    var jobTemplateId = GetJobTemplateId();
    var job = {
        Name: $("#jobName").val(),
        NodeType: $("#jobNode").val(),
        Priority: $("#jobPriorityLabel").text(),
        Tasks: jobTasks,
        TemplateId: jobTemplateId,
    };
    return job;
}
function GetJobTasks() {
    var taskNumber = 1;
    var jobTasks = new Array();
    do {
        var jobTask = GetJobTask(taskNumber);
        if (jobTask != null) {
            jobTasks.push(jobTask);
        }
        taskNumber = taskNumber + 1;
    } while (jobTask != null)
    return jobTasks;
}
function GetJobInputs() {
    var jobInputs = new Array();
    if ($("#mediaAssets").children.length > 0) {
        var mediaAssets = $("#mediaAssets").jstree(true).get_checked();
        for (var i = 0; i < mediaAssets.length; i++) {
            var jobInput = {
                AssetId: mediaAssets[i]
            };
            jobInputs.push(jobInput);
        }
    }
    return jobInputs;
}
function GetJobTemplateId() {
    var jobTemplateId = "";
    var jobName = $("#jobName").val();
    var jobTemplates = document.getElementsByClassName("es-visible");
    for (var i = 0; i < jobTemplates.length; i++) {
        if (jobName == jobTemplates[i].textContent) {
            jobTemplateId = jobTemplates[i].attributes["value"].value;
        }
    }
    return jobTemplateId;
}
function AddJobTaskLink(workflowTable, taskRowIndex) {
    var taskLinkRow = workflowTable.insertRow(taskRowIndex);
    var rowCell0 = taskLinkRow.insertCell(0);
    var rowCell1 = taskLinkRow.insertCell(1);
    taskLinkRow.className = "mediaWorkflowTaskLink";
    rowCell1.innerHTML = "<hr>";
}
function AddJobTask(taskButton) {
    var workflowTable = document.getElementById("mediaWorkflow");
    var taskRowIndex = GetTaskRowIndex(taskButton);
    var lastTaskRow = workflowTable.rows[taskRowIndex - 1];
    var lastTaskNumber = GetTaskNumber(lastTaskRow);
    var newTaskRow = workflowTable.insertRow(taskRowIndex);
    var newTaskNumber = lastTaskNumber + 1;
    ClearJobTaskWidgets(lastTaskNumber);
    newTaskRow.outerHTML = GetNewTaskRowHtml(lastTaskRow, lastTaskNumber, newTaskNumber);
    AddJobTaskLink(workflowTable, taskRowIndex);
    SetJobTaskParents(newTaskNumber);
    SetJobTaskInputs(newTaskNumber);
    SetJobTaskInputs(lastTaskNumber);
    var mediaProcessor = $("#mediaProcessor" + newTaskNumber)[0];
    SetProcessorConfig(mediaProcessor, newTaskNumber);
    $("#mediaWorkflowTaskRemove").show();
    if (lastTaskNumber == 4) {
        $("#mediaWorkflowTaskAdd").hide();
    }
    ScrollToBottom();
}
function RemoveJobTask(taskButton) {
    var workflowTable = document.getElementById("mediaWorkflow");
    var taskRowIndex = GetTaskRowIndex(taskButton);
    var lastTaskRow = workflowTable.rows[taskRowIndex - 1];
    var lastTaskNumber = GetTaskNumber(lastTaskRow);
    workflowTable.deleteRow(lastTaskRow.rowIndex - 1);
    workflowTable.deleteRow(lastTaskRow.rowIndex);
    $("#mediaWorkflowTaskAdd").show();
    if (lastTaskNumber == 2) {
        $("#mediaWorkflowTaskRemove").hide();
    }
    SetTipVisible("mediaProcessor" + taskNumber, false);
    SetTipVisible("encoderConfigFile" + taskNumber, false);
}