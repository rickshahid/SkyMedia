function EnsureVisibility() {
    $.scrollTo("#mediaWorkflowTaskAdd");
    $.scrollTo("#mediaWorkflowTaskRemove");
}
function GetJob() {
    var job = {
        Name: $("#jobName").val(),
        Priority: $("#jobPriorityLabel").text(),
        NodeType: $("#jobNode").val(),
        NotificationType: $("input[name='jobNotification']:checked").val(),
        Tasks: GetJobTasks()
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
function AddJobTaskLink(workflowTable, taskRowIndex) {
    var taskLinkRow = workflowTable.insertRow(taskRowIndex);
    var rowCell0 = taskLinkRow.insertCell(0);
    var rowCell1 = taskLinkRow.insertCell(1);
    taskLinkRow.className = "mediaWorkflowTaskLink";
    rowCell1.innerHTML = "<hr />";
}
function AddJobTask(taskButton) {
    var workflowTable = document.getElementById("mediaWorkflow");
    var taskRowIndex = GetTaskRowIndex(taskButton);
    var lastTaskRow = workflowTable.rows[taskRowIndex - 1];
    var lastTaskNumber = GetLastTaskNumber(lastTaskRow);
    var newTaskRow = workflowTable.insertRow(taskRowIndex);
    var newTaskNumber = lastTaskNumber + 1;
    ClearJobTaskWidgets(lastTaskNumber);
    newTaskRow.outerHTML = GetNewTaskRowHtml(lastTaskRow, lastTaskNumber, newTaskNumber);
    AddJobTaskLink(workflowTable, taskRowIndex);
    SetJobTaskParents(newTaskNumber);
    SetJobTaskWidgets(newTaskNumber);
    SetJobTaskWidgets(lastTaskNumber);
    var mediaProcessor = $("#mediaProcessor" + newTaskNumber)[0];
    SetProcessorConfig(mediaProcessor, newTaskNumber);
    $("#mediaWorkflowTaskAdd").removeClass("mediaWorkflowTaskButtons1");
    $("#mediaWorkflowTaskAdd").addClass("mediaWorkflowTaskButtons2");
    $("#mediaWorkflowTaskRemove").show();
    if (lastTaskNumber == 4) {
        $("#mediaWorkflowTaskAdd").hide();
        $("#mediaWorkflowTaskRemove").addClass("mediaWorkflowTaskButtons1");
    }
    EnsureVisibility();
}
function RemoveJobTask(taskButton) {
    var workflowTable = document.getElementById("mediaWorkflow");
    var taskRowIndex = GetTaskRowIndex(taskButton);
    var lastTaskRow = workflowTable.rows[taskRowIndex - 1];
    var lastTaskNumber = GetLastTaskNumber(lastTaskRow);
    ValidWorkflowTaskClear(lastTaskNumber);
    workflowTable.deleteRow(lastTaskRow.rowIndex - 1);
    workflowTable.deleteRow(lastTaskRow.rowIndex);
    $("#mediaWorkflowTaskAdd").show();
    $("#mediaWorkflowTaskRemove").removeClass("mediaWorkflowTaskButtons1");
    if (lastTaskNumber == 2) {
        $("#mediaWorkflowTaskAdd").removeClass("mediaWorkflowTaskButtons2");
        $("#mediaWorkflowTaskAdd").addClass("mediaWorkflowTaskButtons1");
        $("#mediaWorkflowTaskRemove").hide();
    }
}
