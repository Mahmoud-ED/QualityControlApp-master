document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("saveButton").addEventListener("click", function () {
        let data = [];
        let rows = document.querySelectorAll(".CompanyQuestionContent tr");

        rows.forEach((row, index) => {
            if (index === 0) return;
            let id = row.querySelector(".row-id").textContent;
            let score = row.querySelector(".row-value").value;
            data.push({ Id: id, Score: score });
        });

        if (data.length === 0) {
            alert("لا توجد بيانات لتحديثها.");
            return;
        }

        fetch('CompanyQuestionContentController/Edit', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(result => {
                console.log(result);
                alert("تم حفظ البيانات بنجاح!");
            })
            .catch(error => {
                //console.error("Error:", error);
                //alert("");
            });
    });
});
