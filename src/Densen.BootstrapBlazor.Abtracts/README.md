抽取 BootstrapBlazor 几个特性 (AutoGenerateClass, AutoGenerateColumn..) 独立包,兼容net framework,基于BootstrapBlazor 8.7.1

使用方法:

1. 安装包或者直接第二步手动编辑.csproj文件
    ```xml
    <ItemGroup>
        <PackageReference Include="Densen.BootstrapBlazor.Abtracts" Version="*" />
    </ItemGroup>
    ```

2. 编辑项目文件.csproj
    ```xml
    <PropertyGroup>
        <Frameworks48>|net462|net48|</Frameworks48>
    </PropertyGroup>

    <ItemGroup Condition="$(Frameworks48.Contains('|$(TargetFramework)|'))">
        <PackageReference Include="Densen.BootstrapBlazor.Abtracts" Version="*" />
    </ItemGroup>
    ```

3. 愉快的使用
    ```xml
    [AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true)]
    public partial class Foo  
    {
        [AutoGenerateColumn(Order = 0, Visible = false)]
        [NotNull]
        public int ID { get; set; }

    }
    ```
