// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 类目项（<c>get_category</c> 的 <c>category_list</c> 元素）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/getAllCategoryName.html"/>。
/// </summary>
/// <remarks>本 DTO 是代小程序「可提交类目」（<c>/wxa/get_category</c>），与开放平台已有的「第三方类目」域（<c>/cgi-bin/wxopen/*</c>）不同，勿混用。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpCategoryItem
{
    /// <summary>一级类目名（官方 <c>first_class</c>）。</summary>
    [JsonPropertyName("first_class")]
    public string? FirstClass { get; set; }

    /// <summary>二级类目名（官方 <c>second_class</c>）。</summary>
    [JsonPropertyName("second_class")]
    public string? SecondClass { get; set; }

    /// <summary>三级类目名（官方 <c>third_class</c>）。</summary>
    [JsonPropertyName("third_class")]
    public string? ThirdClass { get; set; }

    /// <summary>一级类目 ID（官方 <c>first_id</c>，number）。</summary>
    [JsonPropertyName("first_id")]
    public long FirstId { get; set; }

    /// <summary>二级类目 ID（官方 <c>second_id</c>，number）。</summary>
    [JsonPropertyName("second_id")]
    public long SecondId { get; set; }

    /// <summary>三级类目 ID（官方 <c>third_id</c>，number）。</summary>
    [JsonPropertyName("third_id")]
    public long ThirdId { get; set; }
}
