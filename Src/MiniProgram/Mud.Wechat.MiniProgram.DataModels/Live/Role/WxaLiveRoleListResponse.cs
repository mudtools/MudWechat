// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 查询成员列表应答（<c>GET /wxaapi/broadcast/role/getrolelist</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/role-management/api_getrolelist.html"/>。
/// </para>
/// <para>
/// <b>官方返回体表不含 <c>errcode</c> / <c>errmsg</c></b>（只有 <c>total</c> 与 <c>list</c>），
/// 而官方<b>示例</b>里带 <c>"errcode": 0</c> ⇒ 继承 <see cref="WxaResponse"/> 同时兼容两种形态，
/// 判成败只看 <c>errcode</c>。
/// </para>
/// <para>官方分页口径：<c>offset</c> 默认 0；<c>limit</c> <b>最大 30、默认 10</b>（本域唯一「上限 30」的分页）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveRoleListResponse : WxaResponse
{
    /// <summary>总个数（<c>total</c>，官方类型 number）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>角色列表（<c>list</c>，官方类型 objarray），见 <see cref="WxaLiveRoleItem"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaLiveRoleItem>? List { get; set; }
}
