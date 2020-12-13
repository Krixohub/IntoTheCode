// Before running this, build IntoTheCodeDoc.csproj in Debug mode to generate "/doc/classes.txt" file
D:
cd \Kode\IntoTheCode\WebMarkdown
php yellow.php build D:\Kode\webGenerated\IntoTheCode

rem Clean cache
php yellow.php clean cache

pause