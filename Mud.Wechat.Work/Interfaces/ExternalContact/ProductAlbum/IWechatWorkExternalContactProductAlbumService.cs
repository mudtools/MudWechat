// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.ProductAlbum;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块商品图册域公共 SDK
/// （创建 / 获取 / 列表 / 编辑 / 删除商品图册）。
/// <para>
/// 官方对三类应用开放完全一致的 5 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactProductAlbumService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactProductAlbumService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactProductAlbumService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「客户联系 可调用接口的应用」中，第三方 / 代开发应用须具有
/// 「管理商品图册」权限；获取接口可读企业内所有企业级图册，编辑 / 删除仅可操作应用自己创建的图册；
/// 商品图片须经「上传附件资源」接口（attachment_type = 2）获得 media_id。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactProductAlbumService
{
    /// <summary>
    /// 创建商品图册
    /// <para>创建一个商品图册，返回商品 id。</para>
    /// <para>附件仅支持 image 类型且最多 9 个，media_id 须经「上传附件资源」接口（attachment_type = 2）获得。</para>
    /// </summary>
    /// <param name="request">创建请求体（<see cref="AddProductAlbumRequest"/>：description / price / attachments）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品 id（product_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95096"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95131"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96345"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/add_product_album")]
    Task<AddProductAlbumResponse> AddProductAlbumAsync(
        [Body] AddProductAlbumRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品图册
    /// <para>通过商品 id 获取商品图册详情（可获取企业内所有企业级的商品图册）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetProductAlbumRequest"/>：product_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品图册详情（product）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95096"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95131"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96345"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_product_album")]
    Task<GetProductAlbumResponse> GetProductAlbumAsync(
        [Body] GetProductAlbumRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品图册列表
    /// <para>分页获取商品图册列表；cursor + limit 分页（limit 最大 100，默认 50）。</para>
    /// <para>自建应用调用只会返回应用可见范围内用户的情况。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetProductAlbumListRequest"/>：limit / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品图册列表（product_list）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95096"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95131"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96345"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_product_album_list")]
    Task<GetProductAlbumListResponse> GetProductAlbumListAsync(
        [Body] GetProductAlbumListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑商品图册
    /// <para>编辑商品图册内容；除 product_id 外，仅需更新的字段才填。</para>
    /// <para>应用只可修改应用自己创建的商品图册。</para>
    /// </summary>
    /// <param name="request">编辑请求体（<see cref="UpdateProductAlbumRequest"/>：product_id + 更新字段）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>编辑结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95096"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95131"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96345"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/update_product_album")]
    Task<WechatWorkResponse> UpdateProductAlbumAsync(
        [Body] UpdateProductAlbumRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除商品图册
    /// <para>删除指定商品图册。应用只可删除应用自己创建的商品图册。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteProductAlbumRequest"/>：product_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95096"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95131"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96345"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/delete_product_album")]
    Task<WechatWorkResponse> DeleteProductAlbumAsync(
        [Body] DeleteProductAlbumRequest request,
        CancellationToken cancellationToken = default);
}
