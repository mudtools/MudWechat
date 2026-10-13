// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「品牌申请」请求体（<c>POST /wxa/sec/famousbrand/apply</c>）。
/// </summary>
/// <remarks>
/// <para><b>包装键是首字母大写的 <c>Application</c></b>（官方请求体表与示例逐字如此，
/// 是本域唯一一个大写起手的 JSON 键）⇒ <c>JsonPropertyName</c> 必须写 <c>"Application"</c>，
/// 守卫 OS4 锁定该拼写；改成 <c>application</c> 即 <c>10210001</c>「参数错误」。</para>
/// <para><b>官方注意事项要求 <c>Content-Type: application/json</c></b>，且错误码表含 <c>44002</c>「POST内容为空」。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_famousbrandapply.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaFamousBrandApplyRequest
{
    /// <summary>Application（官方类型 <c>object</c>，必填）：申请品牌信息</summary>
    [JsonPropertyName("Application")]
    public WxaFamousBrandApplication? Application { get; set; }
}
