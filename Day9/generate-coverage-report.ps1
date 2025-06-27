# Script để tạo coverage report chi tiết

Write-Host "=== Tạo Coverage Report cho Hệ thống tính lương ==="

# Kiểm tra xem ReportGenerator đã được cài đặt chưa
if (!(Get-Command "reportgenerator" -ErrorAction SilentlyContinue)) {
    Write-Host "Cài đặt ReportGenerator..."
    dotnet tool install -g dotnet-reportgenerator-globaltool
}

# Xóa thư mục TestResults cũ nếu có
if (Test-Path "TestResults") {
    Remove-Item -Recurse -Force "TestResults"
}

# Xóa thư mục CoverageReport cũ nếu có
if (Test-Path "CoverageReport") {
    Remove-Item -Recurse -Force "CoverageReport"
}

Write-Host "Chạy tests với coverage collection..."
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

if ($LASTEXITCODE -eq 0) {
    Write-Host "Tests thành công! Tạo HTML coverage report..."
    
    # Tìm file coverage.cobertura.xml
    $coverageFile = Get-ChildItem -Path "TestResults" -Filter "coverage.cobertura.xml" -Recurse | Select-Object -First 1
    
    if ($coverageFile) {
        Write-Host "Tìm thấy coverage file: $($coverageFile.FullName)"
        
        # Tạo HTML report
        reportgenerator -reports:"$($coverageFile.FullName)" -targetdir:"./CoverageReport" -reporttypes:Html
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Coverage report đã được tạo thành công!"
            Write-Host "Mở file: ./CoverageReport/index.html để xem chi tiết"
            
            # Tự động mở report trong browser
            $reportPath = Join-Path (Get-Location) "CoverageReport\index.html"
            if (Test-Path $reportPath) {
                Start-Process $reportPath
            }
        } else {
            Write-Host "Lỗi khi tạo coverage report" -ForegroundColor Red
        }
    } else {
        Write-Host "Không tìm thấy coverage file" -ForegroundColor Red
    }
} else {
    Write-Host "Tests thất bại!" -ForegroundColor Red
}

Write-Host "\n=== Hoàn thành ==="