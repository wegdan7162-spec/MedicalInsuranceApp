-- ===== تشخيص قيم ClaimStatus الموجودة فعلاً في الداتابيس =====
-- شغّلي هذا في SSMS ثم أخبريني بالنتائج

USE MedicalInsuranceApp1;

-- 1) كل القيم المميزة في TblOutters مع العدد
SELECT
    ISNULL(ClaimStatus, '<<NULL>>') AS ClaimStatus,
    LEN(ClaimStatus)                AS طول_النص,
    COUNT(*)                        AS العدد
FROM TblOutters
WHERE Del IS NULL OR Del = 0
GROUP BY ClaimStatus
ORDER BY COUNT(*) DESC;

-- 2) كل القيم المميزة في TblFriendly مع العدد
SELECT
    ISNULL(ClaimStatus, '<<NULL>>') AS ClaimStatus,
    LEN(ClaimStatus)                AS طول_النص,
    COUNT(*)                        AS العدد
FROM TblFriendly
WHERE Del IS NULL OR Del = 0
GROUP BY ClaimStatus
ORDER BY COUNT(*) DESC;

-- 3) فحص خاص: هل في فراغات زيادة؟
SELECT TOP 20
    Id,
    '|' + ClaimStatus + '|' AS ClaimStatus_مع_حدود,
    LEN(ClaimStatus)          AS الطول
FROM TblOutters
WHERE ClaimStatus LIKE '%غير%'
   OR ClaimStatus LIKE '%مغطاء%'
   OR ClaimStatus LIKE '%مغطى%';
