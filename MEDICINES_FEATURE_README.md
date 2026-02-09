# نظام إدارة الأدوية والسجلات الصحية

## نظرة عامة
تم إضافة نظام شامل لإدارة الأدوية وربطها بالسجلات الصحية للموظفين في تطبيق QualityControlApp.

## الميزات المضافة

### 1. إدارة الأدوية (Medicines Management)
قسم مستقل لإدارة الأدوية المتوفرة في النظام:

#### الوظائف:
- **عرض قائمة الأدوية**: عرض جميع الأدوية مع إمكانية البحث والفلترة والترتيب
- **إضافة دواء جديد**: إضافة دواء بمعلومات كاملة (الاسم، الاسم العلمي، الشركة المصنعة، النوع، إلخ)
- **تعديل الدواء**: تحديث معلومات الدواء
- **حذف الدواء**: حذف الأدوية غير المرتبطة بسجلات صحية
- **تفاصيل الدواء**: عرض معلومات الدواء والسجلات الصحية المرتبطة به

#### الحقول المتاحة:
- اسم الدواء (مطلوب)
- الاسم العلمي (Generic Name)
- الشركة المصنعة
- نوع الدواء (أقراص، شراب، حقن، كبسولات، مرهم، قطرة، بخاخ)
- وصف الدواء
- حالة التوفر (متوفر/غير متوفر)
- ملاحظات

#### الوصول:
```
/Medicines/Index
/Medicines/Create
/Medicines/Edit/{id}
/Medicines/Delete/{id}
/Medicines/Details/{id}
```

### 2. ربط الأدوية بالسجلات الصحية (Health Record Medications)
نظام لربط الأدوية بالسجلات الصحية للموظفين:

#### الوظائف:
- **إضافة دواء للسجل الصحي**: ربط دواء موجود بسجل صحي محدد
- **تعديل معلومات الدواء**: تحديث الجرعة، التكرار، التعليمات
- **حذف الدواء من السجل**: إزالة دواء من سجل صحي
- **عرض تفاصيل الدواء**: عرض معلومات الدواء الموصوف

#### الحقول المتاحة:
- الدواء (اختيار من القائمة)
- الجرعة (مثل: 500mg، 10ml)
- التكرار (مثل: مرتين يومياً، كل 8 ساعات)
- تاريخ البدء
- تاريخ الانتهاء
- تعليمات الاستخدام
- نشط حالياً (Active/Inactive)
- ملاحظات

#### الوصول:
```
/HealthRecordMedications/Create?healthRecordId={id}
/HealthRecordMedications/Edit/{id}
/HealthRecordMedications/Delete/{id}
/HealthRecordMedications/Details/{id}
```

### 3. التكامل مع صفحة تفاصيل الموظف
تم تحديث صفحة تفاصيل الموظف لعرض الأدوية الموصوفة:

#### الميزات:
- عرض الأدوية النشطة لكل سجل صحي
- إضافة دواء جديد مباشرة من صفحة الموظف
- تعديل أو حذف الأدوية الموجودة
- عرض الجرعة والتكرار بشكل واضح

## الملفات المضافة/المعدلة

### Models:
- `Models/Entities/Medicine.cs` - نموذج الدواء
- `Models/Entities/HealthRecordMedication.cs` - نموذج ربط الدواء بالسجل الصحي
- `Models/Entities/HealthRecord.cs` - تم تحديثه لإضافة علاقة الأدوية

### Controllers:
- `Controllers/MedicinesController.cs` - التحكم في الأدوية
- `Controllers/HealthRecordMedicationsController.cs` - التحكم في ربط الأدوية بالسجلات
- `Controllers/EmployeesController.cs` - تم تحديثه لتحميل الأدوية

### Views:
#### Medicines:
- `Views/Medicines/Index.cshtml`
- `Views/Medicines/Create.cshtml`
- `Views/Medicines/Edit.cshtml`
- `Views/Medicines/Delete.cshtml`
- `Views/Medicines/Details.cshtml`

#### HealthRecordMedications:
- `Views/HealthRecordMedications/Create.cshtml`
- `Views/HealthRecordMedications/Edit.cshtml`
- `Views/HealthRecordMedications/Delete.cshtml`
- `Views/HealthRecordMedications/Details.cshtml`

#### Updated:
- `Views/Employees/Details.cshtml` - تم تحديثه لعرض الأدوية

### Database:
- `Models/ApplicationDbContext.cs` - تم إضافة DbSet والعلاقات
- Migration: `AddMedicinesAndHealthRecordMedication`

## قاعدة البيانات

### جدول Medicines:
```sql
- Id (Guid, Primary Key)
- Name (nvarchar(200), Required)
- GenericName (nvarchar(100))
- Manufacturer (nvarchar(100))
- Type (nvarchar(50))
- Description (nvarchar(1000))
- IsAvailable (bit)
- Notes (nvarchar(500))
- Created (datetime2)
- Modified (datetime2)
```

### جدول HealthRecordMedications:
```sql
- Id (Guid, Primary Key)
- HealthRecordId (Guid, Foreign Key, Required)
- MedicineId (Guid, Foreign Key, Required)
- Dosage (nvarchar(100), Required)
- Frequency (nvarchar(100), Required)
- StartDate (datetime2)
- EndDate (datetime2)
- Instructions (nvarchar(500))
- IsActive (bit)
- Notes (nvarchar(500))
- Created (datetime2)
- Modified (datetime2)
```

### العلاقات:
- `HealthRecordMedication` → `HealthRecord` (Many-to-One, Cascade Delete)
- `HealthRecordMedication` → `Medicine` (Many-to-One, Restrict Delete)
- `HealthRecord` → `HealthRecordMedications` (One-to-Many)
- `Medicine` → `HealthRecordMedications` (One-to-Many)

## كيفية الاستخدام

### 1. إضافة دواء جديد للنظام:
1. انتقل إلى `/Medicines/Index`
2. اضغط على "إضافة دواء جديد"
3. املأ المعلومات المطلوبة
4. احفظ

### 2. ربط دواء بسجل صحي:
**الطريقة الأولى (من صفحة الموظف):**
1. انتقل إلى صفحة تفاصيل الموظف
2. في قسم السجل الصحي، اضغط على "إضافة دواء" للسجل المطلوب
3. اختر الدواء وحدد الجرعة والتكرار
4. احفظ

**الطريقة الثانية (مباشرة):**
1. انتقل إلى `/HealthRecordMedications/Create?healthRecordId={id}`
2. املأ المعلومات
3. احفظ

### 3. إدارة الأدوية الموصوفة:
- **التعديل**: اضغط على أيقونة التعديل بجانب الدواء
- **الحذف**: اضغط على أيقونة الحذف
- **التفاصيل**: اضغط على اسم الدواء

## ملاحظات مهمة

### Migration:
تم إنشاء Migration بنجاح ولكن قد تحتاج إلى تطبيقه يدوياً:
```bash
dotnet ef database update
```

إذا واجهت مشكلة في Migration بسبب بيانات موجودة، قد تحتاج إلى:
1. حل مشكلة البيانات القديمة أولاً
2. أو تطبيق Migration على قاعدة بيانات جديدة

### الأمان:
- جميع الصفحات محمية بـ `[Authorize]`
- يتم التحقق من صحة البيانات على مستوى Model و Controller
- لا يمكن حذف دواء مرتبط بسجلات صحية

### الأداء:
- يتم استخدام `Include` و `ThenInclude` لتحميل البيانات المرتبطة
- Pagination متوفر في صفحة قائمة الأدوية
- البحث والفلترة متوفرة

## مثال عملي

### سيناريو: موظف لديه مرض السكري
1. **إضافة الأدوية للنظام:**
   - Metformin 500mg (أقراص)
   - Insulin Novolog (حقن)

2. **إنشاء سجل صحي للموظف:**
   - الموظف: أحمد محمد
   - المرض: السكري
   - تاريخ التشخيص: 2024/01/15

3. **ربط الأدوية بالسجل:**
   - **Metformin 500mg**
     - الجرعة: قرص واحد
     - التكرار: مرتين يومياً
     - التعليمات: يؤخذ بعد الأكل
   
   - **Insulin Novolog**
     - الجرعة: 10 وحدات
     - التكرار: قبل كل وجبة
     - التعليمات: حقن تحت الجلد

## الدعم الفني
للمساعدة أو الإبلاغ عن مشاكل، يرجى التواصل مع فريق التطوير.

---
**تاريخ الإضافة:** 19 أكتوبر 2025
**الإصدار:** 1.0
