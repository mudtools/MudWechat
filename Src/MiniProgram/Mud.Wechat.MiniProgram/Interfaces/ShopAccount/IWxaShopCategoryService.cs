// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopAccount;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 · 全量商品类目」域 SDK（1 端点：获取商品类目；
/// 官方 minishopopencomponent2 分区 → cat 子目录「接入商品前必需接口」）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>；
/// 官方本页无权限集编号、无第三方代调用与云调用声明（空缺留档）。
/// </para>
/// <para>
/// <b>本端点是官方唯一给出「频率性表述」的一支</b>（原文）：全量三级类目数据「回包大小约为 2MB」、
/// 「有严格的频率限制」、推荐「每天调用一次更新商家自身缓存」——<b>但没有</b>「次/秒」「次/天」额度数字 ⇒
/// 注释只留档该表述，宿主据此自建日级缓存，不得凭第三方 SDK 注释补数字。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopAccount", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopCategoryService
{
    /// <summary>
    /// 获取商品类目（<c>POST /shop/cat/get</c>，无请求体；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/cat/get_children_cateogry.html"/>
    /// （官方文件名本身即拼作 <c>get_children_cateogry</c>，含官方笔误 <c>cateogry</c> ⇒ URL 照原文引用，路由仍是 <c>/shop/cat/get</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>全量三级类目及其资质要求（顶层 <c>third_cat_list</c>），见 <see cref="WxaShopThirdCategoryListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，「请求参数说明」为<b>无</b>（示例体 <c>{}</c>）⇒ 不声明请求 DTO。</para>
    /// <para><b>体量陷阱（官方原文）</b>：「该接口拉到的是【全量】三级类目数据，数据回包大小约为 2MB」⇒
    /// 不得在请求路径上同步全量拉取，宿主须自缓存（官方建议每天一次）。</para>
    /// <para><b>本端点是 <see cref="IWxaShopAuditService.AuditCategoryAsync"/> 的前置判据来源</b>：
    /// 官方原文「若该类目资质必填，则新增商品前，必须先通过该类目资质申请接口进行资质申请；
    /// 若该类目资质不需要，则该类目自动拥有，无需申请，如依然调用，会报错 <c>1050011</c>」
    /// ⇒ 先读 <c>qualification_type</c> 再决定是否提审。</para>
    /// <para><b>商品资质是另一支字段</b>（<c>product_qualification_type</c>）：官方原文「若该商品资质必填，
    /// 则新增商品时，带上商品资质字段」⇒ 类目资质与商品资质两条要求互不替代。</para>
    /// <para><b>审核结果不在本页</b>（官方原文「接入类目审核回调，才可获取审核结果」）。</para>
    /// <para><b>列表键是 <c>third_cat_list</c>、无 <c>data</c> 包装</b>，元素十支键全为 <c>*_cat_*</c> / <c>qualification*</c> 形态
    /// ⇒ 守卫 SHOPA5 锁定，与 <c>/shop/account/get_category_list</c> 的 <c>data</c> 六支键不互换。</para>
    /// </remarks>
    [Post("/shop/cat/get")]
    Task<WxaShopThirdCategoryListResponse> GetThirdCategoryListAsync(
        CancellationToken cancellationToken = default);
}
