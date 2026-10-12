// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>批量查询运单信息请求体（<c>POST /cgi-bin/express/business/order/batchget</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_batchgetorder"/>；单次最多 50 单。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressBatchGetOrderRequest
{
    /// <summary>查询单列表（<c>order_list</c>，必填；单次最多 50 单）。</summary>
    [JsonPropertyName("order_list")]
    public List<WxaExpressBatchGetOrderItem>? OrderList { get; set; }
}
