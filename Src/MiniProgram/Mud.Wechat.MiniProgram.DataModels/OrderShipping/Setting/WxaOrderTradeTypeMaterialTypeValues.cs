// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 交易类型变更申请材料类型（<c>material_list[].type</c>，仅图片与视频两类）。
/// </summary>
/// <remarks>
/// <para><b>规模限制</b>（与频率无关）：材料最多 10 个，其中视频最多 3 个；两者均须先经
/// 「新增临时素材」上传换 <c>media_id</c>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_setwxatradetypecgi.html"/></para>
/// </remarks>
public static class WxaOrderTradeTypeMaterialTypeValues
{
    /// <summary>图片（<c>1</c>）。</summary>
    public const int Image = 1;

    /// <summary>视频（<c>2</c>）。</summary>
    public const int Video = 2;
}
