// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopAccount;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 · 推广员」域 SDK（1 端点：获取推广员列表；
/// 官方 minishopopencomponent2 分区 → promoter 子目录「推广员接口」）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>；
/// 官方本页无权限集编号、无第三方代调用与云调用声明（空缺留档）。
/// </para>
/// <para>
/// <b>官方本目录只有这一支</b>：现行目录树「推广员接口」分区下仅「获取推广员列表」一条，
/// 其余带货关系端点（绑定/解绑分享员、分享员数据汇总）在「分享员接口」分区、由并行作业域承载 ⇒
/// 本接口不得跨挂（守卫 SHOPA7 零声明清单）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopAccount", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopPromoterService
{
    /// <summary>
    /// 获取推广员列表（<c>POST /shop/promoter/list</c>；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/promoter/list.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>page</c> 与 <c>page_size</c> 两支均必填），见 <see cref="WxaShopPromoterListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广员列表与总数（顶层平铺 <c>promoters</c> / <c>total_num</c>），见 <see cref="WxaShopPromoterListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方本页无端点级频率数字；规模上限只有
    /// <b>「<c>page_size</c> 不超过 20」</b>与「<c>page</c> 最小填 1」两条，属规模限制、与频率分列。</para>
    /// <para><b>本端点是本域唯一「必带分页入参」的读端点</b>：其余列表端点（<c>get_brand_list</c> /
    /// <c>get_category_list</c> / <c>cat/get</c>）官方均为<b>无请求参数的全量返回</b> ⇒ 不要给它们补分页字段。</para>
    /// <para><b>回包顶层平铺、既无 <c>data</c> 也无游标</b>：翻页只能靠 <c>page</c> 递增与 <c>total_num</c> 比较。</para>
    /// <para><b>官方回包示例不含 <c>errmsg</c></b>（回包表列了但示例省略）⇒ 该键可能缺省，判错须看 <c>errcode</c>。</para>
    /// <para><b>身份体系跨产品</b>：<c>finder_nickname</c> 是<b>视频号</b>昵称、<c>promoter_id</c> 是非定长字符串 id
    /// （官方示例形如 <c>a-****…po</c>，<b>不得数值化</b>），<c>promoter_openid</c> 属用户表示标识
    /// ⇒ 后者不得写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/shop/promoter/list")]
    Task<WxaShopPromoterListResponse> GetPromoterListAsync(
        [Body] WxaShopPromoterListRequest request,
        CancellationToken cancellationToken = default);
}
