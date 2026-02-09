# 🏗️ AirPortRequest Structure Diagram

## 📊 Entity Relationships

```
┌─────────────────────────────────────────────────────────────┐
│                    AirPortRequest                           │
├─────────────────────────────────────────────────────────────┤
│  Core Information                                          │
│  ├── Department (string, Required)                         │
│  ├── Email (string, Required, EmailAddress)               │
│  ├── SenderName (string, Required)                        │
│  └── CompanyName (string, Required)                       │
├─────────────────────────────────────────────────────────────┤
│  Time Information                                          │
│  ├── EntryTime (DateTime, Required)                       │
│  ├── RequestTime (DateTime, Required)                     │
│  ├── FlightDate (DateTime, Required)                      │
│  └── LandingTakeoffTime (string, Required)                │
├─────────────────────────────────────────────────────────────┤
│  Aircraft Information                                      │
│  ├── AircraftType (string, Required)                      │
│  ├── AircraftRegistration (string, Required)              │
│  └── CallSign (string, Required)                          │
├─────────────────────────────────────────────────────────────┤
│  Flight Details                                            │
│  ├── FlightPath (string, Required)                        │
│  ├── FlightPurpose (string, Required)                     │
│  └── EntryExitPoints (string, Required)                   │
├─────────────────────────────────────────────────────────────┤
│  Status & Approval                                         │
│  ├── RequestStatus (string, Required)                     │
│  ├── ApproverUserId (string?, Optional)                   │
│  └── Notes (string?, Optional)                            │
├─────────────────────────────────────────────────────────────┤
│  Extended Information                                      │
│  ├── PilotName (string?, Optional)                        │
│  ├── FlightNumber (string?, Optional)                     │
│  ├── EntryPoint (string?, Optional)                       │
│  ├── ExitPoint (string?, Optional)                        │
│  ├── EstimatedEntryTime (DateTime?, Optional)             │
│  ├── EstimatedExitTime (DateTime?, Optional)              │
│  ├── CargoDetails (string?, Optional)                     │
│  ├── CrewCount (int?, Optional)                           │
│  └── CrewNationalities (string?, Optional)                │
└─────────────────────────────────────────────────────────────┘
                                │
                                │ 1:Many
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                AirPortRequestFiles                         │
├─────────────────────────────────────────────────────────────┤
│  ├── FileName (string, Required)                          │
│  ├── FilePath (string, Required)                          │
│  ├── AirPortRequestId (Guid?, Optional)                   │
│  ├── FileTypeId (Guid, Required)                          │
│  ├── Inspect (string?, Optional)                          │
│  └── Nots (string?, Optional)                             │
└─────────────────────────────────────────────────────────────┘
                                │
                                │ Many:1
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                    FileType                                │
├─────────────────────────────────────────────────────────────┤
│  ├── Id (Guid, Primary Key)                               │
│  ├── Name (string)                                        │
│  └── Description (string?)                                │
└─────────────────────────────────────────────────────────────┘

                                │
                                │ Many:1
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                ApplicationUser                             │
├─────────────────────────────────────────────────────────────┤
│  ├── Id (string, Primary Key)                             │
│  ├── UserName (string)                                    │
│  ├── Email (string)                                       │
│  └── ... (other user properties)                          │
└─────────────────────────────────────────────────────────────┘
```

## 🔄 Data Flow Diagram

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   User      │───▶│  Controller │───▶│  Database   │
│  Interface  │    │             │    │             │
└─────────────┘    └─────────────┘    └─────────────┘
       │                   │                   │
       │                   │                   │
       ▼                   ▼                   ▼
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│ Validation  │    │ File Upload │    │   Email     │
│ (Client &   │    │   System    │    │ Notification│
│  Server)    │    │             │    │             │
└─────────────┘    └─────────────┘    └─────────────┘
```

## 🎯 Controller Actions Flow

```
┌─────────────────┐
│      Index      │
│  (List & Filter)│
└─────────┬───────┘
          │
          ▼
┌─────────────────┐
│     Create      │
│  (GET & POST)   │
└─────────┬───────┘
          │
          ▼
┌─────────────────┐
│     Details     │
│  (View Request) │
└─────────┬───────┘
          │
          ▼
┌─────────────────┐
│      Edit       │
│  (Update Data)  │
└─────────┬───────┘
          │
          ▼
┌─────────────────┐
│  ChangeStatus   │
│ (Admin Action)  │
└─────────────────┘
```

## 📋 Field Categories

### **Required Fields (15):**
1. Department
2. EntryTime
3. RequestTime
4. Email
5. SenderName
6. CompanyName
7. FlightDate
8. AircraftType
9. AircraftRegistration
10. CallSign
11. FlightPath
12. LandingTakeoffTime
13. FlightPurpose
14. EntryExitPoints
15. RequestStatus

### **Optional Fields (11):**
1. Notes
2. ApproverUserId
3. PilotName
4. FlightNumber
5. EntryPoint
6. ExitPoint
7. EstimatedEntryTime
8. EstimatedExitTime
9. CargoDetails
10. CrewCount
11. CrewNationalities

## 🔧 Validation Rules

### **Data Type Validations:**
- **Email**: EmailAddress attribute
- **CrewCount**: Range(0, int.MaxValue)
- **Required Fields**: Required attribute with custom messages

### **Business Logic Validations:**
- **RequestStatus**: Must be "Pending", "Approved", or "Rejected"
- **File Upload**: File type and size validation
- **Time Validation**: EntryTime should be before FlightDate

## 🎨 UI Components

### **Form Sections:**
1. **Basic Information**: Department, Email, Sender, Company
2. **Time Information**: Entry, Request, Flight dates
3. **Aircraft Details**: Type, Registration, Call Sign
4. **Flight Information**: Path, Purpose, Entry/Exit points
5. **Additional Details**: Pilot, Crew, Cargo information
6. **File Attachments**: Multiple file upload with types
7. **Status Management**: Admin-only status changes

### **List View Features:**
1. **Filtering**: By email, date range, status
2. **Sorting**: By request time, flight date
3. **KPIs**: Status distribution, daily counts
4. **Actions**: View, Edit, Delete, Change Status

## 🚀 Future Enhancements

### **Planned Improvements:**
1. **Dropdown Fields**: Convert text inputs to dropdowns
2. **File Security**: Enhanced file validation
3. **Pagination**: For large datasets
4. **Real-time Updates**: Using SignalR
5. **Mobile Support**: Responsive design
6. **API Integration**: External system connectivity

---

**Last Updated**: January 2025  
**Version**: 1.0.0  
**Status**: Structure Analysis Complete
