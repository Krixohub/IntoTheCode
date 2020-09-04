D:
cd \Kode\IntoTheCode\WebMarkdown
php yellow.php build D:\Kode\webGenerated\IntoTheCode

rem Clean cache
php yellow.php clean cache

rem Copy web assembly
md D:\Kode\webGenerated\IntoTheCode\webasm
xcopy "D:\Kode\IntoTheCode\CSharp\TestWebApp\bin\Release\netstandard2.1\publish\wwwroot" "D:\Kode\webGenerated\IntoTheCode\webasm\" /A /S /D /Y


pause