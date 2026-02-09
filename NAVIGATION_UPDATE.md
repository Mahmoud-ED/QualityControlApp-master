# تحديث القائمة الجانبية - Navigation Update

## التحديثات المضافة في _LayoutDashboard.cshtml

تم إضافة قوائم جديدة في القائمة الجانبية (Sidebar) لتسهيل الوصول إلى الميزات الجديدة:

---

## 1️⃣ قائمة الموظفين (Employees)

### الموقع في القائمة:
بعد قائمة "Airport Management"

### التفاصيل:
```html
<li class="nav-item">
    <a class="nav-link" asp-controller="Employees" asp-action="Index">
        <span class="icon-bg"><i class="mdi mdi-account-tie menu-icon"></i></span>
        <span class="menu-title">Employees</span>
    </a>
</li>
```

### الوظيفة:
- رابط مباشر لصفحة قائمة الموظفين
- يفتح `/Employees/Index`
- الأيقونة: `mdi-account-tie` (رجل بربطة عنق)

---

## 2️⃣ قائمة إدارة الصحة (Health Management)

### الموقع في القائمة:
بعد قائمة "Employees"

### التفاصيل:
```html
<li class="nav-item">
    <a class="nav-link" data-bs-toggle="collapse" href="#healthManagementMenu">
        <span class="icon-bg"><i class="mdi mdi-hospital-box menu-icon"></i></span>
        <span class="menu-title">Health Management</span>
        <i class="menu-arrow mdi mdi-chevron-down"></i>
    </a>
    <div class="collapse" id="healthManagementMenu">
        <ul class="nav flex-column sub-menu">
            <li class="nav-item">
                <a class="nav-link" asp-controller="Medicines" asp-action="Index">
                    <span class="icon-bg ms-3"><i class="mdi mdi-pill menu-icon"></i></span>
                    Medicines
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" asp-controller="ChronicDisease" asp-action="Index">
                    <span class="icon-bg ms-3"><i class="mdi mdi-heart-pulse menu-icon"></i></span>
                    Chronic Diseases
                </a>
            </li>
        </ul>
    </div>
</li>
```

### القوائم الفرعية:

#### 1. Medicines (الأدوية)
- **الرابط:** `/Medicines/Index`
- **الأيقونة:** `mdi-pill` (حبة دواء)
- **الوظيفة:** إدارة الأدوية المتوفرة في النظام

#### 2. Chronic Diseases (الأمراض المزمنة)
- **الرابط:** `/ChronicDisease/Index`
- **الأيقونة:** `mdi-heart-pulse` (نبض القلب)
- **الوظيفة:** إدارة الأمراض المزمنة

---

## 📋 ترتيب القوائم في Sidebar

```
1. Dashboard
2. Over Sight
3. All Operators
4. Overflight Requests
5. Reports
6. Landing Requests (قائمة منسدلة)
7. Create Appointment
8. Companies
9. Questions & Categories (قائمة منسدلة)
10. Airport Management (قائمة منسدلة)
11. ⭐ Employees (جديد)
12. ⭐ Health Management (جديد - قائمة منسدلة)
    - Medicines
    - Chronic Diseases
13. Users (قائمة منسدلة)
14. Settings (قائمة منسدلة)
15. Send Email
16. My Profile
17. Log Out
```

---

## 🎨 الأيقونات المستخدمة

تم استخدام أيقونات من مكتبة **Material Design Icons (MDI)**:

| القائمة | الأيقونة | الكود |
|---------|----------|-------|
| Employees | 👔 | `mdi-account-tie` |
| Health Management | 🏥 | `mdi-hospital-box` |
| Medicines | 💊 | `mdi-pill` |
| Chronic Diseases | 💓 | `mdi-heart-pulse` |

---

## 🔗 المسارات (Routes)

### الموظفين:
```
/Employees/Index          - قائمة الموظفين
/Employees/Details/{id}   - تفاصيل الموظف (تحتوي على السجلات الصحية والأدوية)
```

### الأدوية:
```
/Medicines/Index          - قائمة الأدوية
/Medicines/Create         - إضافة دواء جديد
/Medicines/Edit/{id}      - تعديل دواء
/Medicines/Delete/{id}    - حذف دواء
/Medicines/Details/{id}   - تفاصيل الدواء
```

### الأمراض المزمنة:
```
/ChronicDisease/Index     - قائمة الأمراض المزمنة
```

### ربط الأدوية بالسجلات:
```
/HealthRecordMedications/Create?healthRecordId={id}  - إضافة دواء لسجل صحي
/HealthRecordMedications/Edit/{id}                   - تعديل دواء موصوف
/HealthRecordMedications/Delete/{id}                 - حذف دواء من سجل
```

---

## 🚀 كيفية الاستخدام

### للوصول إلى قائمة الموظفين:
1. افتح القائمة الجانبية
2. اضغط على "Employees"
3. ستفتح صفحة قائمة الموظفين

### للوصول إلى إدارة الأدوية:
1. افتح القائمة الجانبية
2. اضغط على "Health Management"
3. اختر "Medicines" من القائمة المنسدلة
4. ستفتح صفحة إدارة الأدوية

### للوصول إلى الأمراض المزمنة:
1. افتح القائمة الجانبية
2. اضغط على "Health Management"
3. اختر "Chronic Diseases" من القائمة المنسدلة

---

## 📱 Responsive Design

القوائم تعمل بشكل كامل على:
- 💻 Desktop
- 📱 Mobile
- 📱 Tablet

القائمة الجانبية قابلة للطي على الشاشات الصغيرة.

---

## ✅ التحقق من التحديثات

للتأكد من أن التحديثات تعمل بشكل صحيح:

1. ✅ تشغيل التطبيق
2. ✅ تسجيل الدخول
3. ✅ فتح القائمة الجانبية
4. ✅ التحقق من وجود قائمة "Employees"
5. ✅ التحقق من وجود قائمة "Health Management"
6. ✅ فتح القوائم الفرعية والتأكد من الروابط

---

**تاريخ التحديث:** 19 أكتوبر 2025
