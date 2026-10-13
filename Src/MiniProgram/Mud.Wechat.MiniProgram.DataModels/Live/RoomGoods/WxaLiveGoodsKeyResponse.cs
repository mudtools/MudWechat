// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 直播挂件全局 KEY 应答（<c>GET /wxaapi/broadcast/goods/getkey</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getdefault_goodskey.html"/>。
/// </para>
/// <para>
/// <b>读写字段名不同源</b>：设置用 <c>goodsKey</c>（<see cref="WxaLiveGoodsKeyRequest"/>）、
/// 读取回 <c>vendorGoodsKey</c> —— 官方两页如此，不得按「同名往返」假设。
/// 本端点<b>无任何业务请求参数</b>（官方请求参数表只有 <c>access_token</c>）⇒ 接口方法不声明参数体。
/// </para>
/// <para>调用额度：500 次/天。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveGoodsKeyResponse : WxaResponse
{
    /// <summary>全局 KEY 数组（<c>vendorGoodsKey</c>，官方类型 array）。</summary>
    [JsonPropertyName("vendorGoodsKey")]
    public List<string>? VendorGoodsKey { get; set; }
}
