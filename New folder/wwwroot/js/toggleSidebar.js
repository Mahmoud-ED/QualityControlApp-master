document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("Sidebar");
    const content = document.getElementById("Content");
    const toggleSidebarSizeButton = document.getElementById("toggleSidebarSizeButton");

    if (window.innerWidth > 768) {
        sidebar.style.display = "block";
    }

    //const sidebarState = sessionStorage.getItem("sidebarState");
    //if (sidebarState === "hidden") {
    //    sidebar.style.display = "none";
    //}

    toggleSidebarSizeButton.addEventListener("click", function () {

        window.scrollTo({
            top: 0,
            behavior: "smooth"
        });
        window.scrollTo(0, 0);


        if (sidebar.style.display === "none" || sidebar.style.display === "") {

            sidebar.style.display = "block";
            content.classList.remove("col-lg-12");
            content.classList.add("col-lg-9");

            //sessionStorage.removeItem("sidebarState");
        }
        else {
            sidebar.style.display = "none";
            content.classList.remove("col-lg-9");
            content.classList.add("col-lg-12");
            //sessionStorage.setItem("sidebarState", "hidden");
        }

    });
});


//// اختبار عرض الشاشة باستخدام media query
//const mediaQuery = window.matchMedia('(min-width: 992px)');

//// تابع لمعالجة التغييرات في media query
//const handleScreenSizeChange = function (mediaQuery) {
//    const sidebar = document.getElementById("Sidebar");
//    if (mediaQuery.matches) {
//        sidebar.style.display = "none"; // إذا كانت شاشة العرض أكبر من 992 بيكسل، جعل العنصر مخفي
//    } else {
//        sidebar.style.display = ""; // إذا كانت شاشة العرض أصغر من 992 بيكسل، جعل العنصر مرئي
//    }
//};

//// استدعاء التابع مع mediaQuery الحالية
//handleScreenSizeChange(mediaQuery);

//// استماع للتغييرات في mediaQuery واستدعاء التابع عندما يتغير الحالة
//mediaQuery.addListener(handleScreenSizeChange);
