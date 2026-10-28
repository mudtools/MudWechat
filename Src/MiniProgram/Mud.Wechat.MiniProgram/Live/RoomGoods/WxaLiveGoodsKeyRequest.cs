// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 直播挂件设置全局 KEY 请求体（<c>POST /wxaapi/broadcast/goods/setkey</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_setdefault_goodskey.html"/>。
/// </para>
/// <para>
/// <b>官方注意事项原文「key 必须为字符串数组」</b>⇒ <see cref="GoodsKey"/> 建为
/// <c>List&lt;string&gt;?</c>（本页「请求体」表类型列为 array，示例 <c>["productId", "sid"]</c>），
/// 不得按数字数组建模。
/// </para>
/// <para>
/// <b>页面名与路由的第三组别名</b>：官方文件名 <c>api_setdefault_goodskey</c>、接口英文名
/// <c>setdefault_goodskey</c>（小写下划线），路由 <c>goods/setkey</c>，请求字段 <c>goodsKey</c>
/// —— 四处命名各不相同，本 SDK 对外只暴露路由与字段口径。
/// </para>
/// <para>调用额度：500 次/天。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveGoodsKeyRequest
{
    /// <summary>商品 KEY 数组（<c>goodsKey</c>，必填；官方注意事项「key 必须为字符串数组」）。</summary>
    [JsonPropertyName("goodsKey")]
    public List<string>? GoodsKey { get; set; }
}
