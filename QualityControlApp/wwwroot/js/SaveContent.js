$(document).ready(function () {
    $('#saveButton').on('click', function () {
        var updatedData = [];
        var companyQuestionId = $('#companyQuestionId').val(); // تأكد من وجود هذا الحقل وقيمته

        $('.question-row').each(function () {
            var questionId = $(this).data('id');
            var rowData = {
                QuestionId: questionId
            };

            if ($('input.row-score', this).length > 0) { // حالة ViewBag.Type == "Old"
                rowData.Score = parseFloat($('input.row-score', this).val());
            } else { // حالة ViewBag.Type != "Old"
                rowData.Inspect = $('select.row-inspect', this).val();
                var notsContainer = $('div.nots-container', this);
                if (notsContainer.is(':visible')) {
                    rowData.Notes = $('textarea.nots-text', notsContainer).val();
                    rowData.Level = $('select.nots-level', $(this).next('td')).val(); // الوصول إلى خلية المستوى
                } else {
                    rowData.Notes = null;
                    rowData.Level = null;
                }
            }

            updatedData.push(rowData);
        });

        console.log("Data to be sent:", JSON.stringify(updatedData)); // فحص البيانات في وحدة التحكم

        if (updatedData.length > 0) {
            $.ajax({
                url: '/CompanyQuestion/UpdateCompanyQuestionContent?Id=' + companyQuestionId,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(updatedData),
                success: function (response) {
                    if (response.success) {
                        alert('تم حفظ التغييرات بنجاح!');
                        // يمكنك هنا تحديث واجهة المستخدم إذا لزم الأمر
                    } else {
                        alert('حدث خطأ أثناء حفظ التغييرات: ' + response.message);
                    }
                },
                error: function (xhr, status, error) {
                    alert('حدث خطأ في طلب AJAX: ' + error);
                }
            });
        } else {
            alert('لا توجد بيانات للتحديث.');
        }
    });
});