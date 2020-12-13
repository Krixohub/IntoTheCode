rem Before running this, generate md files with Build "IntoTheCode.bat"

rem Copy web assembly
md D:\Kode\webGenerated\IntoTheCode\webasm
xcopy "D:\Kode\IntoTheCode\CSharp\TestWebApp\bin\Debug\netstandard2.1\publish\wwwroot" "D:\Kode\webGenerated\IntoTheCode\webasm\" /A /S /D /Y


pause