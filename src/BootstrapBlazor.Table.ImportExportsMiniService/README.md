BootstrapBlazor Table 数据导入导出服务扩展包

Table 导出UI

```
    <ExportButtonDropdownTemplate>
        <h6 class="dropdown-header">当前页数据</h6>
        <div class="dropdown-item" @onclick="_=>ExportExcelAsync(mainTable.Rows)">
            <i class="fas fa-file-excel"></i>
            <span>Excel</span>
        </div>
        <div class="dropdown-item" @onclick="_=>ExportWordAsync(mainTable.Rows)">
            <i class="fas fa-file-word"></i>
            <span>Word</span>
        </div>
        <div class="dropdown-item" @onclick="_=>ExportHtmlAsync(mainTable.Rows)">
            <i class="fa-brands fa-html5"></i>
            <span>Html</span>
        </div>
        <div class="dropdown-item" @onclick="_=>ExportPDFAsync(mainTable.Rows)">
            <i class="fas fa-file-pdf"></i>
            <span>PDF</span>
        </div>
        <div class="dropdown-divider"></div>
        <h6 class="dropdown-header">全部数据</h6>
        <div class="dropdown-item" @onclick="_=>ExportExcelAsync(GetAllItems())">
            <i class="fas fa-file-excel"></i>
            <span>Excel</span>
        </div>
        <div class="dropdown-item" @onclick="_=>ExportWordAsync(GetAllItems())">
            <i class="fas fa-file-word"></i>
            <span>Word</span>
        </div>
        <div class="dropdown-item" @onclick="_=>ExportHtmlAsync(GetAllItems())">
            <i class="fa-brands fa-html5"></i>
            <span>Html</span>
        </div>
        <div class="dropdown-item" @onclick="_=>ExportPDFAsync(GetAllItems())">
            <i class="fas fa-file-pdf"></i>
            <span>PDF</span>
        </div>
    </ExportButtonDropdownTemplate>
```

C# 代码

```
        [Inject]
        [NotNull]
        ImportExportsService? ImportExportsService { get; set; }

        private async Task<bool> ExportExcelAsync(IEnumerable<TItem> items) => await ExportAutoAsync(items, ExportType.Excel);
        private async Task<bool> ExportPDFAsync(IEnumerable<TItem> items) => await ExportAutoAsync(items, ExportType.Pdf);
        private async Task<bool> ExportWordAsync(IEnumerable<TItem> items) => await ExportAutoAsync(items, ExportType.Word);
        private async Task<bool> ExportHtmlAsync(IEnumerable<TItem> items) => await ExportAutoAsync(items, ExportType.Html);

        private async Task<bool> ExportAutoAsync(IEnumerable<TItem> items, ExportType exportType = ExportType.Excel)
        {
            if (items == null || !items.Any())
            {
                await ToastService.Error("提示", "无数据可导出");
                return false;
            }
            var option = new ToastOption()
            {
                Category = ToastCategory.Information,
                Title = "提示",
                Content = $"导出正在执行,请稍等片刻...",
                IsAutoHide = false
            };
            // 弹出 Toast
            await ToastService.Show(option);
            await Task.Delay(100);


            // 开启后台进程进行数据处理
            await Export(items?.ToList(), exportType);

            // 关闭 option 相关联的弹窗
            await option.Close();

            // 弹窗告知下载完毕
            await ToastService.Show(new ToastOption()
            {
                Category = ToastCategory.Success,
                Title = "提示",
                Content = $"导出成功,请检查数据",
                IsAutoHide = false
            });
            return true;

        }

```

完整示例看源码工程