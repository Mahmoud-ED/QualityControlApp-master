# حل مشكلة Migration - RequestStatus

## المشكلة
```
Conversion failed when converting the nvarchar value 'Approved' to data type int.
```

## السبب
Migration `UpdateAirPortRequestValidation` يحاول تحويل عمود `RequestStatus` من `nvarchar` (نص) إلى `int` (رقم)، ولكن توجد بيانات قديمة تحتوي على قيم نصية مثل "Approved", "Pending", "Rejected".

## الحلول المتاحة

### الحل 1: تنظيف قاعدة البيانات وإعادة إنشائها (الأسهل)
⚠️ **تحذير:** سيتم حذف جميع البيانات!

```powershell
# في Package Manager Console
Drop-Database
Update-Database
```

أو باستخدام SQL:
```sql
DROP DATABASE [YourDatabaseName]
```
ثم:
```powershell
Update-Database
```

---

### الحل 2: تحديث البيانات يدوياً قبل Migration (الأفضل للحفاظ على البيانات)

#### الخطوة 1: تحديث البيانات في قاعدة البيانات
قم بتنفيذ هذا الـ SQL Script في قاعدة البيانات:

```sql
-- تحديث القيم النصية إلى أرقام
UPDATE AirPortRequests
SET RequestStatus = 
    CASE RequestStatus
        WHEN 'Pending' THEN '0'
        WHEN 'Approved' THEN '1'
        WHEN 'Rejected' THEN '2'
        WHEN 'UnderReview' THEN '3'
        ELSE '0'  -- القيمة الافتراضية
    END
WHERE TRY_CAST(RequestStatus AS INT) IS NULL;
```

#### الخطوة 2: تطبيق Migration
```powershell
Update-Database
```

---

### الحل 3: إنشاء Migration مخصص لتحويل البيانات

#### الخطوة 1: إنشاء Migration جديد
```powershell
Add-Migration FixRequestStatusDataConversion
```

#### الخطوة 2: تعديل Migration المُنشأ
افتح الملف الجديد وأضف هذا الكود في `Up`:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // تحويل البيانات القديمة
    migrationBuilder.Sql(@"
        UPDATE AirPortRequests
        SET RequestStatus = 
            CASE RequestStatus
                WHEN 'Pending' THEN '0'
                WHEN 'Approved' THEN '1'
                WHEN 'Rejected' THEN '2'
                WHEN 'UnderReview' THEN '3'
                ELSE '0'
            END
        WHERE TRY_CAST(RequestStatus AS INT) IS NULL;
    ");
}
```

#### الخطوة 3: تطبيق Migration
```powershell
Update-Database
```

---

### الحل 4: التراجع عن Migration المشكل ثم إعادة تطبيقه

#### الخطوة 1: التراجع إلى Migration قبل المشكلة
```powershell
Update-Database -Migration InitialCreate
```

#### الخطوة 2: حذف Migration المشكل
```powershell
Remove-Migration
```

#### الخطوة 3: إعادة إنشاء Migration بشكل صحيح
قم بتعديل الكود لإضافة تحويل البيانات، ثم:
```powershell
Add-Migration UpdateAirPortRequestValidation
Update-Database
```

---

## الحل الموصى به (للحفاظ على البيانات)

### الخطوات:

1. **افتح SQL Server Management Studio** أو أي أداة لإدارة قاعدة البيانات

2. **نفذ هذا الـ Script:**
```sql
-- تحقق من القيم الموجودة
SELECT DISTINCT RequestStatus FROM AirPortRequests;

-- تحديث القيم
UPDATE AirPortRequests
SET RequestStatus = 
    CASE RequestStatus
        WHEN 'Pending' THEN '0'
        WHEN 'Approved' THEN '1'
        WHEN 'Rejected' THEN '2'
        WHEN 'UnderReview' THEN '3'
        WHEN 'InProgress' THEN '4'
        ELSE '0'
    END
WHERE TRY_CAST(RequestStatus AS INT) IS NULL;

-- تحقق من النتيجة
SELECT DISTINCT RequestStatus FROM AirPortRequests;
```

3. **في Package Manager Console:**
```powershell
Update-Database
```

---

## قيم Enum المتوقعة

حسب الكود، القيم المتوقعة هي:

```csharp
public enum RequestStatus
{
    Pending = 0,        // قيد الانتظار
    Approved = 1,       // موافق عليه
    Rejected = 2,       // مرفوض
    UnderReview = 3,    // قيد المراجعة
    InProgress = 4      // قيد التنفيذ (إذا كان موجود)
}
```

---

## ملاحظات مهمة

1. ✅ **قبل أي عملية:** قم بعمل Backup لقاعدة البيانات
2. ✅ **تحقق من القيم:** استخدم `SELECT DISTINCT RequestStatus FROM AirPortRequests` لمعرفة القيم الموجودة
3. ✅ **اختبر على قاعدة بيانات تجريبية** أولاً إذا أمكن
4. ⚠️ **لا تحذف البيانات** إلا إذا كنت متأكداً أنك لا تحتاجها

---

## بعد حل المشكلة

بعد تطبيق أحد الحلول بنجاح، يمكنك تطبيق Migration الأدوية الجديد:

```powershell
Update-Database
```

سيتم تطبيق:
- `AddMedicinesAndHealthRecordMedication`
- `NewMedecaltable`

وسيعمل التطبيق بشكل كامل! ✅
