// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 设置成员角色应答（<c>POST /wxaapi/broadcast/role/addrole</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/role-management/api_getrolelistdw.html"/>。
/// </para>
/// <para>
/// <b>官方返回体表只有 <c>codeurl</c> 一个字段</b>（无 <c>errcode</c> / <c>errmsg</c>），且其说明为条件语义
/// 「如果主播未实名认证，需要先前往"小程序直播"小程序进行实名验证」⇒ 仅在设置<b>主播</b>角色且未实名时出现；
/// 官方错误码 <c>400002</c> 亦标注「仅设置主播角色时可能出现」。成功示例反而是
/// <c>{ "errcode": 0, "errmsg": "ok" }</c>（不含 codeurl）⇒ 两字段都可能缺省，宿主须按「有无 codeurl 决定是否引导实名」处理。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveAddRoleResponse : WxaResponse
{
    /// <summary>实名认证引导链接（<c>codeurl</c>，官方原文：主播未实名认证时需前往「小程序直播」小程序验证）。</summary>
    [JsonPropertyName("codeurl")]
    public string? CodeUrl { get; set; }
}
