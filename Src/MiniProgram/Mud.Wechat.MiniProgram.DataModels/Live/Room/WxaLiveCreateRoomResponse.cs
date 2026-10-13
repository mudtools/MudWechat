// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 创建直播间应答（<c>POST /wxaapi/broadcast/room/create</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_createroom.html"/>。
/// </para>
/// <para>
/// <b>官方返回体表只列 <c>roomId</c> 与 <c>qrcode_url</c></b>：成功响应示例亦无 <c>errcode</c>
/// ⇒ 继承 <see cref="WxaResponse"/> 后 <c>errcode</c> 缺省 0 即成功，判错仍以 <see cref="WxaResponse.IsSuccess"/> 为准。
/// </para>
/// <para>
/// <b>必须持久化 <see cref="RoomId"/></b>：本域其余端点（编辑 / 删除 / 推流地址 / 商品导入等）都以房间 ID 定位，
/// 与「添加并提审商品须保存商品 ID 与审核单 ID」同属官方明示的「丢失即无法调用其他接口」形态。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveCreateRoomResponse : WxaResponse
{
    /// <summary>房间 ID（<c>roomId</c>，官方类型 number），本域其余端点的定位键。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }

    /// <summary>「小程序直播」小程序码（<c>qrcode_url</c>，官方字段名为<b>下划线</b>拼写）。</summary>
    [JsonPropertyName("qrcode_url")]
    public string? QrcodeUrl { get; set; }
}
