// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductSku;

/// <summary>SPU 草稿状态（<c>spu/get</c>、<c>spu/get_list</c>、<c>spu/search</c> 的 <c>edit_status</c>）。</summary>
public static class WxaProductSpuEditStatuses
{
    /// <summary>0 初始值。</summary>
    public const int Initial = 0;

    /// <summary>1 编辑中。</summary>
    public const int Editing = 1;

    /// <summary>2 审核中。</summary>
    public const int UnderReview = 2;

    /// <summary>3 审核失败。</summary>
    public const int ReviewFailed = 3;

    /// <summary>4 审核成功。</summary>
    public const int ReviewSucceeded = 4;
}
