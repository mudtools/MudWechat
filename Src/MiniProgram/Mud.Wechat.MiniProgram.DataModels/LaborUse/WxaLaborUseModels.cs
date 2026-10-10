// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.LaborUse;

/// <summary>推送用工消息请求体（<c>POST /cgi-bin/message/wxopen/employeerelationmsg/send</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>laboruse/api_sendemployeerelationmsg.html</c>。</para>
/// <para>
/// 用工关系场景下向<b>已建立用工关联</b>的用户推送模板消息；<c>template_id</c> 为
/// 用工类目专属模板 ID。<c>data</c> 为推送数据 <b>JSON 字符串</b>（官方类型即 string，
/// 由键值对 <c>"key": {"value": ...}</c> 组成）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "LaborUse")]
public class WxaSendEmployeeRelationMessageRequest
{
    /// <summary>模板 ID（<c>template_id</c>，必填；用工类目专属模板）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>接收消息的用户 OpenId（<c>touser</c>，必填）。</summary>
    [JsonPropertyName("touser")]
    public string? ToUserOpenId { get; set; }

    /// <summary>小程序页面路径（<c>page</c>，跳转链接）。</summary>
    [JsonPropertyName("page")]
    public string? PagePath { get; set; }

    /// <summary>推送数据 JSON 字符串（<c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }
}

/// <summary>解绑用工关系请求体（<c>POST /wxa/business/unbinduserb2cauthinfo</c>）。</summary>
/// <remarks>官方文档：<c>laboruse/api_unbinduserb2cauthinfo.html</c>。<c>openid_list</c> 为待解绑用户列表（必填，非空）。</remarks>
[HttpJsonSerializable(SerializerClassName = "LaborUse")]
public class WxaUnbindUserB2CAuthInfoRequest
{
    /// <summary>解绑用户的 OpenId 列表（<c>openid_list</c>，必填；上限以官方页面为准）。</summary>
    [JsonPropertyName("openid_list")]
    public List<string>? OpenIdList { get; set; }
}