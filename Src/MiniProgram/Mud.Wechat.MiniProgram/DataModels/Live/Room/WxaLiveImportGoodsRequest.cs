// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 导入商品（把商品库商品加入直播间）请求体（<c>POST /wxaapi/broadcast/room/addgoods</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_importgoods.html"/>。
/// </para>
/// <para>
/// <b>前置：商品须已审核通过</b>。官方错误码 <c>300025</c>「商品审核未通过」与 <c>300024</c>「商品不存在」
/// 表明本端点只接受 <c>/wxaapi/broadcast/goods/*</c> 商品管理面里审核通过（<c>audit_status = 2</c>）的商品。
/// </para>
/// <para>
/// <b>数量上限</b>：官方错误码 <c>300033</c>「添加商品超过直播间上限」——上限值官方页面未给出数字，
/// 故本 SDK <b>不做本地校验</b>、不写死常量（照录原则），由官方返回码兜住。
/// </para>
/// <para>
/// <b><c>ids</c> 官方类型为 numarray</b>（数字数组）：与「获取商品的信息与审核状态」的 <c>goods_ids</c> 同形，
/// 亦与商品排序里的 <c>goods[].goodsId</c>（对象数组、示例值为字符串）不同 ⇒ 各自照录。
/// </para>
/// <para>调用额度：10000 次/天。官方错误码另含 <c>200002</c>（入参错误）/ <c>300022</c> / <c>300023</c> / <c>300027</c>（导入商品失败）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveImportGoodsRequest
{
    /// <summary>商品 ID 数组（<c>ids</c>，必填；官方类型 numarray，「数组列表，可传入多个，里面填写商品 ID」）。</summary>
    [JsonPropertyName("ids")]
    public List<long>? Ids { get; set; }

    /// <summary>房间 ID（<c>roomId</c>，必填）。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }
}
