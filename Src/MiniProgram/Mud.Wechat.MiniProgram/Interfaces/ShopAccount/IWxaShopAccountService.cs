// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopAccount;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 · 商家入驻信息」域 SDK（4 端点：获取品牌列表 / 获取类目列表 /
/// 获取商家信息 / 更新商家信息；官方 minishopopencomponent2 分区 → account 子目录「商家入驻接口」）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// 本域全部 4 页官方均以 <c>https://api.weixin.qq.com/shop/xxx?access_token=xxxxxxxxx</c> 形式给出请求地址，
/// 无第二把凭据参数。
/// </para>
/// <para>
/// <b>官方本页无权限集编号、无第三方代调用与云调用声明（诚实留档）</b>：minishopopencomponent2 文档树
/// 与 <c>miniprogram/dev/server/API/*</c> 树不同，逐页<b>不含</b>「适用范围 / 权限集 / 云调用」小节 ⇒
/// 注释一律留档该空缺，<b>不得</b>凭第三方 SDK 注释或记忆补数（同域判例见直播 / 短剧 / 虚拟支付三域的留档口径）。
/// </para>
/// <para>
/// <b>官方本域无端点级频率数字</b>：4 页均未给「次/秒」「次/天」类数字 ⇒ 同上留档。
/// </para>
/// <para>
/// <b>两支「列表」与两支「信息」的读写不对称</b>：<c>get_brand_list</c> / <c>get_category_list</c> /
/// <c>get_info</c> 三支官方「请求参数说明」均为<b>无</b>（请求体 <c>{}</c>）⇒ 本域三支不声明请求 DTO，
/// 只有 <c>update_info</c> 有请求体；<c>update_info</c> 是账号级唯一值的<b>覆盖式写入</b>，非补丁语义。
/// </para>
/// <para>
/// <b>敏感面</b>：<c>service_agent_phone</c> 与默认退货地址的收件人姓名 / 手机号 / 详细地址均属个人信息，
/// <b>不得</b>写入日志、遥测或异常消息（见 <see cref="WxaShopAddressInfo"/>）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopAccount", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopAccountService
{
    /// <summary>
    /// 获取商家品牌列表（<c>POST /shop/account/get_brand_list</c>；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/account/brand_list.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>品牌列表（顶层 <c>data</c> 数组），见 <see cref="WxaShopBrandListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体，官方「请求参数说明」为<b>无</b>（示例体 <c>{}</c>）⇒ 本端点不声明请求 DTO；
    /// 官方本页无端点级频率数字。</para>
    /// <para><b>只回「已申请成功」的品牌</b>（官方正文原文「获取已申请成功的品牌列表」）：
    /// 提审中与被驳回的品牌不在列表内 ⇒ 不能拿本端点当审核进度查询用（进度走 <c>/shop/audit/result</c>）。</para>
    /// <para><b>元素键名是 <c>brand_wording</c>（不是 <c>brand_name</c>）</b>、列表键是 <c>data</c>（不是 <c>data.list</c>）
    /// ⇒ 守卫 SHOPA5 锁定。</para>
    /// </remarks>
    [Post("/shop/account/get_brand_list")]
    Task<WxaShopBrandListResponse> GetBrandListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商家类目列表（<c>POST /shop/account/get_category_list</c>；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/account/category_list.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>类目列表（顶层 <c>data</c> 数组，六支 <c>*_cat_*</c> 键），见 <see cref="WxaShopCategoryListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，「请求参数说明」为<b>无</b>（示例体 <c>{}</c>）⇒ 不声明请求 DTO；官方本页无端点级频率数字。</para>
    /// <para><b>本端点 ≠ <c>POST /shop/cat/get</c></b>：本页只回<b>本商家已申请成功</b>的类目，
    /// <c>/shop/cat/get</c> 回<b>全量</b>三级类目及其资质要求；两者列表键名亦不同（<c>data</c> vs <c>third_cat_list</c>）。</para>
    /// <para><b>官方正文标题句自带重复字</b>（原文「获取已申请成功的类类目列表」）⇒ 照录留档，不作语义修正。</para>
    /// </remarks>
    [Post("/shop/account/get_category_list")]
    Task<WxaShopCategoryListResponse> GetCategoryListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商家信息（<c>POST /shop/account/get_info</c>；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/account/get_info.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客服配置与默认退货地址（<c>data</c> 对象包装），见 <see cref="WxaShopAccountInfoResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，「请求参数说明」为<b>无</b>（示例体 <c>{}</c>）⇒ 不声明请求 DTO；官方本页无端点级频率数字。</para>
    /// <para><b>回包 <c>data</c> 是对象</b>，与另两支列表端点的 <c>data</c> 数组同键不同形 ⇒ 守卫 SHOPA5 锁定，勿合并建模。</para>
    /// <para><b>必填集与写侧不同</b>（官方逐页参数表原文差异）：本页把 <c>default_receiving_address</c> 的
    /// <c>province</c> / <c>city</c> / <c>town</c> 标「否」（选填），而 <see cref="UpdateInfoAsync"/> 同一结构标「是」⇒
    /// 读回的对象可能缺这三段，「读—改—写」回环不得假定键对称。</para>
    /// <para><b>敏感面</b>：客服联系电话与退货地址（收件人姓名 / 手机号 / 详细地址）禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/account/get_info")]
    Task<WxaShopAccountInfoResponse> GetInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新商家信息（<c>POST /shop/account/update_info</c>，覆盖式写入；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/account/update_info.html"/>。
    /// </summary>
    /// <param name="request">请求体（字段平铺于顶层、<b>无 <c>data</c> 包装</b>），见 <see cref="WxaShopAccountUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（官方回包示例即此两支）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；<c>service_agent_type</c> 必填，退货地址子字段六支必填、仅 <c>country</c> 选填；
    /// 官方本页无端点级频率数字。</para>
    /// <para><b>覆盖式写入、非幂等于「增量」</b>：本端点写的是账号级唯一值，重复提交即整份覆盖 ⇒
    /// 宿主须先 <see cref="GetInfoAsync"/> 取当前值再整份回写，不得只带增量字段当补丁。</para>
    /// <para><b>官方本页首段是一条业务副作用警示</b>（原文）：「如果开启了小程序客服之后，配置了消息回调，
    /// 则微信不会向客服助手发送消息，如果希望不处理客服消息，请参照将消息转发到客服一节，将消息继续转发到客服系统」
    /// ⇒ 本端点提交客服配置会改变消息路由，<b>errcode=0 不代表客服链路仍可用</b>。</para>
    /// <para><b>官方类型列写 <c>AddressInfo</c>、get_info 页写 <c>Address</c></b>（同一结构两套名字，键名一致）⇒ 本仓两侧共用
    /// <see cref="WxaShopAddressInfo"/>，守卫 SHOPA5 锁定八个子键名。</para>
    /// </remarks>
    [Post("/shop/account/update_info")]
    Task<WxaResponse> UpdateInfoAsync(
        [Body] WxaShopAccountUpdateRequest request,
        CancellationToken cancellationToken = default);
}
