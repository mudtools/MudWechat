// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「提交审核」页面项（<c>submit_audit</c> 的 <c>item_list</c> 元素）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/submitAudit.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpAuditItem
{
    /// <summary>功能页面路径（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>功能标签（官方 <c>tag</c>）。</summary>
    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

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
    public long? FirstId { get; set; }

    /// <summary>二级类目 ID（官方 <c>second_id</c>，number）。</summary>
    [JsonPropertyName("second_id")]
    public long? SecondId { get; set; }

    /// <summary>三级类目 ID（官方 <c>third_id</c>，number）。</summary>
    [JsonPropertyName("third_id")]
    public long? ThirdId { get; set; }

    /// <summary>页面标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}
