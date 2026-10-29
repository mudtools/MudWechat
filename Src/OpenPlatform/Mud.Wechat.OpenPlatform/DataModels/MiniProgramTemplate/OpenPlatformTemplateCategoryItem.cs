// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate;

/// <summary>
/// 标准模板类目信息项（<c>gettemplatelist</c> 应答 <c>template_list[].category_list</c> 数组元素；普通模板返回空数组）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/template-management/getTemplateList.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramTemplate")]
public class OpenPlatformTemplateCategoryItem
{
    /// <summary>小程序页面（官方字段名 <c>address</c>，string）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>小程序标签，空格分隔，至多 10 个、每个至多 20 字（官方字段名 <c>tag</c>，string）。</summary>
    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    /// <summary>一级类目名称（官方字段名 <c>first_class</c>，string）。</summary>
    [JsonPropertyName("first_class")]
    public string? FirstClass { get; set; }

    /// <summary>二级类目名称（官方字段名 <c>second_class</c>，string）。</summary>
    [JsonPropertyName("second_class")]
    public string? SecondClass { get; set; }

    /// <summary>三级类目名称（官方字段名 <c>third_class</c>，string）。</summary>
    [JsonPropertyName("third_class")]
    public string? ThirdClass { get; set; }

    /// <summary>小程序页面标题，至多 32 字（官方字段名 <c>title</c>，string）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>一级类目 id（官方字段名 <c>first_id</c>，number）。</summary>
    [JsonPropertyName("first_id")]
    public long FirstId { get; set; }

    /// <summary>二级类目 id（官方字段名 <c>second_id</c>，number）。</summary>
    [JsonPropertyName("second_id")]
    public long SecondId { get; set; }

    /// <summary>三级类目 id（官方字段名 <c>third_id</c>，number）。</summary>
    [JsonPropertyName("third_id")]
    public long ThirdId { get; set; }
}
