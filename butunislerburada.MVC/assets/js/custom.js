$("body").on("click", ".SendContact", function () {
    
    var form = $("#contactForm")[0];
    var formData = new FormData(form);

    $.ajax({
        url: '/Page/Contact',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        type: 'POST',
        success: function (data) {

            $("#LoadingPanel").modal("hide");
            $("#ShowStatusInfo").show();

            if (data.IsSuccess != 0) {

                $("#ShowStatusInfo").removeClass("alert-danger");
                $("#ShowStatusInfo").addClass("alert-success");
                $("#ShowStatusInfoText").html("<i class='fa fa-check pr10'></i> <strong>Başarılı</strong> " + data.Message);

            }
            else {

                $("#ShowStatusInfo").removeClass("alert-success");
                $("#ShowStatusInfo").addClass("alert-danger");
                $("#ShowStatusInfoText").html("<i class='fa fa-exclamation pr10'></i> <strong>Hata</strong> " + data.Message);

            }
        },
    });

});