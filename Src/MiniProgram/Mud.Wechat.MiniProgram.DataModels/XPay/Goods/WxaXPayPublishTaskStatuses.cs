// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 批量任务总状态取值（官方 <c>Res.status</c> 枚举表）。
/// </summary>
/// <remarks>
/// <para><b>「任务级 status」与「条目级 publish_status」是两个口径</b>：status 描述整条任务是否跑完，<c>publish_status</c> 描述单个道具成败 ⇒ 只有 <c>2</c>（失败或部分失败）才需要逐条目看 <c>errmsg</c>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_publish_goods.html"/></para>
/// </remarks>
public static class WxaXPayPublishTaskStatuses
{
    /// <summary>无任务在运行（<c>0</c>）</summary>
    public const int None = 0;

    /// <summary>任务运行中（<c>1</c>）</summary>
    public const int Running = 1;

    /// <summary>发布失败或部分失败（发布任务已经完成）（<c>2</c>）</summary>
    public const int FailedOrPartialFailed = 2;

    /// <summary>发布成功（<c>3</c>）</summary>
    public const int Success = 3;
}
