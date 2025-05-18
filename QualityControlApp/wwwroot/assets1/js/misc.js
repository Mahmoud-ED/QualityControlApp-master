(function ($) {
    'use strict';
    $(function () {
        var body = $('body');
        var sidebar = $('.sidebar');

        // --- BEGIN MODIFIED SECTION ---

        // Function to normalize a path: remove trailing slash, convert to lowercase
        function normalizePath(path) {
            if (!path) return "";
            return path.toLowerCase().replace(/\/$/, "");
        }

        // Function to get the "effective" href from an ASP.NET Core generated link
        // It tries to reconstruct a comparable path: /Controller/Action
        function getComparableHref(element) {
            var href = element.attr('href');
            if (!href) return null;

            // Simple case: href is already a relative path like /Controller/Action
            if (href.startsWith('/')) {
                // Remove query string and hash
                return normalizePath(href.split('?')[0].split('#')[0]);
            }
            // This part is tricky if hrefs are not consistently formed.
            // For now, we'll assume they are full or relative paths.
            // If you have more complex scenarios (e.g., base URLs), this might need adjustment.
            return normalizePath(href); // Fallback for other cases
        }


        function addActiveClass(element, currentPath) {
            var elementHref = getComparableHref(element);

            if (!elementHref) return; // Skip if no valid href

            // Exact match for the full path (Controller/Action)
            if (elementHref === currentPath) {
                var parentNavItem = element.parents('.nav-item').last();
                parentNavItem.addClass('active');

                // If the active link is inside a collapsible sub-menu, open it
                if (element.parents('.sub-menu').length) {
                    var parentCollapse = element.closest('.collapse');
                    if (parentCollapse.length && !parentCollapse.hasClass('show')) {
                        // Use Bootstrap's API to show the collapse if it's not a direct click
                        // For initial load, directly adding 'show' is fine.
                        // If this function is also called on click, using .collapse('show') is better.
                        parentCollapse.addClass('show');
                    }
                    element.addClass('active'); // Make the specific sub-menu link active
                }
                return true; // Match found
            }

            // Handle cases where the link is to a controller and the current path is an action within that controller
            // e.g., link is /Admin, current path is /Admin/Index or /Admin/Settings
            // This is a broader match, so apply it carefully.
            // For simplicity, we will primarily rely on exact match above.
            // If you need parent controller highlighting, this logic needs refinement.
            // For example, if elementHref is "/admin" and currentPath is "/admin/index"
            // if (currentPath.startsWith(elementHref + '/') || currentPath === elementHref) {
            //    // ... logic to activate parent if it's a "group" link ...
            // }


            return false; // No match
        }

        // Get current path and normalize it
        var currentPath = normalizePath(location.pathname);

        // If currentPath is just "/" (root), and you have a link to "Index" in your "Home" or "Admin" controller
        // you might need to specifically map it.
        // For example, if your default route is /Admin/Index for the root:
        if (currentPath === "/" || currentPath === "") {
            // Assuming your default page is something like /Admin/Index
            // Adjust this if your default controller/action is different
            var defaultController = "admin"; // or "home" or whatever your default is
            var defaultAction = "index";
            currentPath = normalizePath(`/${defaultController}/${defaultAction}`);
            // Special case for root links that might just be "/"
            // but your JS logic expects something like /controller/action
        }


        // --- Cleanup and Activation ---
        sidebar.find('.nav-item.active').removeClass('active');
        sidebar.find('.nav-link.active').removeClass('active');
        // Don't hide all collapses initially by default if some should be open based on URL
        // sidebar.find('.collapse.show').removeClass('show'); // We'll open them selectively

        var activeLinkFound = false;
        $('.nav li a', sidebar).each(function () {
            var $this = $(this);
            if (addActiveClass($this, currentPath)) {
                activeLinkFound = true;
                // Optional: If you want to stop after the first exact match for performance.
                // However, if a submenu link and its parent nav-item both match aspects of the URL
                // and need 'active', then don't return false here. The current logic handles this.
            }
        });

        // If no exact link was found, you might want to activate a parent menu
        // if the current page is a sub-page of it.
        // This logic can get complex and depends on your desired behavior.
        // For example, if on /Controller/Edit, you might want /Controller/Index to also show as active (parent).
        // The current `addActiveClass` focuses on exact matches or direct parent sub-menus.


        // --- END MODIFIED SECTION ---


        //Close other submenu in sidebar on opening any (Accordion effect)
        sidebar.on('show.bs.collapse', '.collapse', function (e) {
            // Find all sibling .collapse elements within the same .nav parent
            // or if it's a top-level .nav-item's .collapse, find others at that level.
            var $thisCollapse = $(this);
            var $parentNavItem = $thisCollapse.closest('.nav-item');

            if ($parentNavItem.length) {
                // Hide other .collapse elements that are siblings of this .nav-item's .collapse
                $parentNavItem.siblings().find('.collapse.show').collapse('hide');
            } else {
                // If it's not directly within a .nav-item (less common for main sidebar structure)
                // Fallback to original logic, but be cautious.
                sidebar.find('.collapse.show').not($thisCollapse).not($thisCollapse.parents('.collapse')).collapse('hide');
            }
        });


        $(".aside-toggler").on("click", function () {
            $(".mail-sidebar,.chat-list-wrapper").toggleClass("menu-open");
        });


        applyStyles();
        function applyStyles() {
            if (!body.hasClass("rtl")) {
                if ($('.settings-panel .tab-content .tab-pane.scroll-wrapper').length) {
                    new PerfectScrollbar('.settings-panel .tab-content .tab-pane.scroll-wrapper');
                }
                if ($('.chats').length) {
                    new PerfectScrollbar('.chats');
                }
                if (body.hasClass("sidebar-fixed")) {
                    if ($('#sidebar .nav').length) {
                        new PerfectScrollbar('#sidebar .nav');
                    }
                }
            }
        }

        $('[data-bs-toggle="minimize"], [data-toggle="minimize"]').on("click", function () { // Added data-bs-toggle for Bootstrap 5
            if ((body.hasClass('sidebar-toggle-display')) || (body.hasClass('sidebar-absolute'))) {
                body.toggleClass('sidebar-hidden');
            } else {
                body.toggleClass('sidebar-icon-only');
            }
        });

        $(".form-check label,.form-radio label").append('<i class="input-helper"></i>');

        $("#fullscreen-button").on("click", function toggleFullScreen() {
            // ... (fullscreen logic - seems okay) ...
            if ((document.fullScreenElement !== undefined && document.fullScreenElement === null) || (document.msFullscreenElement !== undefined && document.msFullscreenElement === null) || (document.mozFullScreen !== undefined && !document.mozFullScreen) || (document.webkitIsFullScreen !== undefined && !document.webkitIsFullScreen)) {
                if (document.documentElement.requestFullScreen) {
                    document.documentElement.requestFullScreen();
                } else if (document.documentElement.mozRequestFullScreen) {
                    document.documentElement.mozRequestFullScreen();
                } else if (document.documentElement.webkitRequestFullScreen) {
                    document.documentElement.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
                } else if (document.documentElement.msRequestFullscreen) {
                    document.documentElement.msRequestFullscreen();
                }
            } else {
                if (document.cancelFullScreen) {
                    document.cancelFullScreen();
                } else if (document.mozCancelFullScreen) {
                    document.mozCancelFullScreen();
                } else if (document.webkitCancelFullScreen) {
                    document.webkitCancelFullScreen();
                } else if (document.msExitFullscreen) {
                    document.msExitFullscreen();
                }
            }
        });

        // Ensure Bootstrap 5 collapse works correctly with data-toggle (if you still have those)
        // Bootstrap 5 uses data-bs-toggle. If you have older `data-toggle="collapse"` ensure they work.
        // The provided HTML uses `data-bs-toggle`, so this might not be strictly necessary unless mixing versions.

    });
})(jQuery);