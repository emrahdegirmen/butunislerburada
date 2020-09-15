$(function () {
    $(".select2").select2();
    $(".textEditor").wysihtml5();
});

$('.fileUpload input[type="file"]').on('change', function () {
    $('.fileUpload input[type="text"]').val($(this).val());
});

$('.fileUpload span').on('click', function () {
    $('.fileUpload input[type="text"]').val($('.fileUpload input[type="file"]').val());
});



$("body").on("click", ".DeleteFormData", function () {

    var thisDataID = $(this).attr("data-id");
    var thisDataUrl = $(this).attr("data-url");
    var thisDataCurrentDiv = $(this).attr("data-current-div");

    $("#ConfirmationPanelYes").attr("data-id", thisDataID);
    $("#ConfirmationPanelYes").attr("data-url", thisDataUrl);
    $("#ConfirmationPanelYes").attr("data-current-div", thisDataCurrentDiv);

    $("#ConfirmationPanel").modal();
});

$("body").on("click", "#ConfirmationPanelYes", function () {

    var thisDataID = $(this).attr("data-id");
    var thisDataUrl = $(this).attr("data-url");
    var thisDataCurrentDiv = $(this).attr("data-current-div");

    $.ajax({
        type: "POST",
        url: thisDataUrl,
        success: function (data) {

            $("#ShowStatusInfo").show();
            $("#ConfirmationPanel").modal("hide");

            if (thisDataCurrentDiv != null) {

                $("#ShowStatusInfo").addClass("alert-success");
                $("#ShowStatusInfoText").html("<i class='fa fa-check pr10'></i> <strong>Başarılı !</strong> " + data.Message);
                $("#" + thisDataCurrentDiv + "-" + thisDataID).hide();

            }
            else {

                $("#ShowStatusInfo").addClass("alert-danger");
                $("#ShowStatusInfoText").html("<i class='fa fa-check pr10'></i> <strong>Hata !</strong> " + data.Message);

            }

        },
    });
});

$("body").on("change", ".ChangeStatus", function () {

    var thisDataUrl = $(this).attr("data-url");
    var thisDataID = $(this).attr("data-id");
    var thisStatusID = 0;

    if ($(this).prop("checked") == true) {
        thisStatusID = 1;
        $("#statusLabel-" + thisDataID).html("Aktif");
    }
    else {
        $("#statusLabel-" + thisDataID).html("Pasif");
    }

    $.ajax({
        type: "POST",
        url: thisDataUrl + "?Id=" + thisDataID + "&StatusId=" + thisStatusID,
        success: function (data) {

        },
    });
});

$("body").on("change", ".ChangeBotStatus", function () {

    var thisDataUrl = $(this).attr("data-url");
    var thisDataID = $(this).attr("data-id");
    var thisStatusID = 0;

    if ($(this).prop("checked") == true) {
        thisStatusID = 1;
        $("#statusBotLabel-" + thisDataID).html("Evet");
    }
    else {
        $("#statusBotLabel-" + thisDataID).html("Hayır");
    }

    $.ajax({
        type: "POST",
        url: thisDataUrl + "?Id=" + thisDataID + "&StatusId=" + thisStatusID,
        success: function (data) {

        },
    });
});


$("body").on("click", ".SendForm", function () {

    $("#LoadingPanel").modal();

    var DataFormID = $(this).attr("data-form-id");
    var DataPostUrl = $(this).attr("data-post-url");
    var DataReturnUrl = $(this).attr("data-return-url");

    var form = $("#" + DataFormID)[0];
    var formData = new FormData(form);

    if ($("#ImgFilePath").get(0) != null) {
        var ImgFilePath = $("#ImgFilePath").get(0).files;
        formData.set("ImgFilePath", ImgFilePath[0]);
    }

    $.ajax({
        url: DataPostUrl,
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        type: 'POST',
        success: function (data) {

            $("#LoadingPanel").modal("hide");
            $("#ShowStatusInfo").show();

            if (data.IsSuccess != 0) {

                window.location.href = DataReturnUrl;

            }
            else {

                $("#ShowStatusInfo").addClass("alert-danger");
                $("#ShowStatusInfoText").html("<i class='fa fa-exclamation pr10'></i> <strong>Hata</strong> " + data.Message);

            }
        },
    });

});


$("body").on("change", "#item_PopularityPoint", function () {
        
    var thisDataID = this.value;
    var thisStatusID = $(this).children("option").filter(":selected").text();

    $.ajax({
        type: "POST",
        url: "/Singer/ChangePopularityPoint?Id=" + thisDataID + "&StatusId=" + thisStatusID,
        success: function (data) {

        },
    });
});
