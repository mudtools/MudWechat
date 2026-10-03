// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

/// <summary>
/// 获客助手组件获取获客链接详情响应体（<c>/cgi-bin/externalcontact/customer_acquisition/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class GetAcquisitionComponentLinkDetailResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置获客链接信息（组件版仅返回链接名称与访问地址，不含自建版的配置字段）。
    /// </summary>
    [JsonPropertyName("link")]
    public AcquisitionComponentLinkInfo? Link { get; set; }
}

/// <summary>
/// 获客助手组件链接信息（组件版子集：仅 <c>link_name</c> 与 <c>url</c> 两个字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class AcquisitionComponentLinkInfo
{
    /// <summary>
    /// 获取或设置获客链接名称。
    /// </summary>
    [JsonPropertyName("link_name")]
    public string? LinkName { get; set; }

    /// <summary>
    /// 获取或设置获客链接访问地址（形如 <c>https://work.weixin.qq.com/ca/xxxxxx?comp_scene=xxxxxx</c>，
    /// 组件版链接需携带 <c>comp_scene</c> 参数方计入组件数据）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
