// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.ShopCoupon;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 V2 · 优惠券本体」域接口（7 端点：领券事件确认 / 添加 / 更新 / 详情 / 列表 /
/// 库存更新 / 状态更新；官方 <c>minishopopencomponent2/API/coupon</c> 分区的优惠券族）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// 自定义交易组件 V2 的 <c>/shop/*</c> 面全部以<b>小程序自身</b> <c>access_token</c> 调用，
/// 与小店线（微信小店 / 视频号）的 <c>/channels/ec/*</c> 令牌链无关（守卫 SC7 锁定两线不交叉）。
/// </para>
/// <para>
/// <b>准入前置</b>：官方 <c>coupon/confirm</c> 页原文「必须先调用该 API 接通领券功能之后，才能调用创建优惠券等其他 API」
/// ⇒ <see cref="ConfirmCouponCallbackAsync"/> 是本域其余 6 支端点的<b>硬前置</b>，且回调地址「以最晚确认回调领券事件的地址为准」。
/// </para>
/// <para>
/// <b>状态机（官方 <c>update_coupon_status</c> 页原文）</b>：优惠券初始状态为 <c>1</c>（未生效、编辑中），
/// 更新为 <c>2</c>（生效）后<b>不可回退为 1</b>；<c>3</c>（已过期）与 <c>4</c>（已作废）<b>只可由 2 更新</b>；
/// 且「当且仅当优惠券状态为 1 时可更新」⇒ <see cref="UpdateCouponAsync"/> 在生效后必然失败，
/// 库存与状态两支是唯一仍可用的写入口。<b>官方本域无删除优惠券端点</b>（作废只能走状态 4）。
/// </para>
/// <para>
/// <b>金额与折扣单位</b>：<c>discount_fee</c> / <c>product_price</c> / <c>tradein_info.price</c> 单位为<b>分</b>；
/// <c>discount_num</c> 是<b>千分制折扣数</b>（5.1 折填 <c>5100</c>）。官方硬校验：<c>discount_fee</c> 不得超过 200 元、
/// <c>discount_num</c> 不得低于 2 折，商品券适用商品优惠后不得低于原价 2 折、店铺满减券不得低于价格门槛 2 折。
/// </para>
/// <para>
/// <b>库存口径（最易误读的一支）</b>：<c>receive_info.total_num</c>（总发放量）与
/// <c>receive_info.limit_num_one_person</c>（个人限领）官方原文明写「<b>只做展示</b>，领券回调时接入方判断有无超领」
/// ⇒ 微信侧<b>不</b>据其扣减；真正的余量/发放量由 <see cref="UpdateCouponStockAsync"/> 的
/// <c>stock_info.issued_num</c>（剩余量）与 <c>receive_num</c>（发放量）承载，两者同为<b>覆盖式全量写</b>、非增量补丁。
/// </para>
/// <para>
/// <b>覆盖式写入</b>：<see cref="AddCouponAsync"/> 与 <see cref="UpdateCouponAsync"/> 的 <c>coupon</c> 请求体是
/// <b>整体覆盖</b>（非 JSON Patch），漏传字段即失去原值 ⇒ 标准流程为 <c>get</c> 读回、改后 <c>update</c> 写回。
/// </para>
/// <para>
/// <b>敏感面</b>：应答与请求里的 <c>appid</c>、<c>promote_info.finder.nickname</c>（视频号昵称）为商家侧标识信息；
/// 同域用户券侧（<see cref="IWxaShopUserCouponService"/>）的 <c>openid</c> 属用户标识 ⇒ 一律<b>不得写入日志、遥测或异常消息</b>。
/// </para>
/// <para>
/// <b>调用限制（诚实留档）</b>：本域 7 页官方正文均<b>未给</b>端点级频率数字，也<b>未给</b>权限集编号、云调用方法名与
/// 第三方平台代调用说明（2026-10-18 逐页取证：正文关键词「频率 / 权限集 / 第三方 / 云调用」零命中）⇒ 注释一律留档该空缺，
/// 不得凭第三方 SDK 注释或记忆补数；「一张商品券最多关联 128 个商品」「<c>page_size</c> 最大 200」是<b>规模</b>限制，与频率分列。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopCoupon", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopCouponService
{
    /// <summary>
    /// 商家确认回调领券事件（<c>POST /shop/coupon/confirm</c>，官方请求体为空对象 <c>{}</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/coupon/confirm.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 空请求体；本域官方无端点级频率数字。作用是「接通领券功能」——
    /// 微信收到本请求后<b>反向回调</b>商家固定消息地址推送 <c>open_product_receive_coupon</c> 事件，
    /// 商家须按官方格式回包（<c>ret_code</c> / <c>request_id</c> / <c>out_coupon_id</c> / <c>out_user_coupon_id</c>）确认，
    /// 否则<b>无法调用优惠券其它任何 API</b>。
    /// </para>
    /// <para>
    /// <b>成功响应不代表接通</b>：官方回包只有 <c>errcode</c>，真正的确认结果取决于商家回调侧对推送的应答
    /// ⇒ 宿主须自行验证后续 <see cref="AddCouponAsync"/> 是否可用，不得以 <c>errcode=0</c> 当作落地证据。
    /// </para>
    /// <para>
    /// <b>幂等但后写覆盖</b>：官方原文「为防止回调冲突……最终回调地址，<b>以最晚确认回调领券事件的地址为准</b>」
    /// ⇒ 多实例/多环境重复调用会把领券回调地址改到「最后一次」所用的消息地址上，属危险副作用，不得放进自动重试管道。
    /// </para>
    /// <para>官方请求体无任何业务字段 ⇒ 本方法不带 <c>[Body]</c> 参数（守卫 SC3 锁定），全仓同形态先例见 <c>Charge</c> 域。</para>
    /// </remarks>
    [Post("/shop/coupon/confirm")]
    Task<WxaResponse> ConfirmCouponCallbackAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加优惠券（<c>POST /shop/coupon/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/coupon/add_coupon.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>coupon</c> 包装的单对象），见 <see cref="WxaShopCouponAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（官方回包示例只有 <c>errcode</c>，<b>不回</b>生成的券标识）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体，<b>恰一支顶层键 <c>coupon</c></b>（不是 <c>add_coupon</c>、也不是平铺字段）；
    /// 本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b><c>out_coupon_id</c> 是商家侧自拟主键</b>：官方无「服务端生成券 id」回包字段，详情 / 更新 / 库存 / 状态
    /// 四支全部以 <c>out_coupon_id</c> 定位 ⇒ 宿主必须自拟并保证跨批次唯一、不可复用；
    /// 重复添加同 id 属覆盖式写入语义，<b>非幂等安全</b>，不得放进自动重试管道。
    /// </para>
    /// <para>
    /// <b><c>promote_type</c> 两处都要传</b>（官方参数表逐行）：<c>coupon.promote_type</c> 与
    /// <c>coupon.coupon_info.promote_info.promote_type</c> 同为必填，取值目前仅 <c>4</c>（视频号直播）；
    /// 且 <c>coupon_info.promote_info.finder.nickname</c>（推广视频号昵称）官方标为必填。
    /// </para>
    /// <para>
    /// <b><c>type</c> 决定必填集</b>：1 商品条件折扣券 / 2 商品满减券 / 3 商品统一折扣券 / 4 商品直减券 /
    /// 5 商品换购券 / 6 商品买赠券 / 101 店铺条件折扣券 / 102 店铺满减券 / 103 店铺统一折扣券 / 104 店铺直减券 ⇒
    /// 商品券必需 <c>discount_condition.out_product_ids</c>（<b>最多 128 个商品</b>）、换购券必需 <c>tradein_info</c>、
    /// 买赠券必需 <c>buyget_info</c>；<c>valid_type</c> 为 1/2/3 时分别必需 <c>start_time+end_time</c> /
    /// <c>valid_day_num</c> / <c>valid_second</c>。
    /// </para>
    /// <para>
    /// <b>金额与折扣单位</b>：<c>product_price</c> / <c>tradein_info.price</c> / <c>discount_fee</c> 单位为<b>分</b>，
    /// <c>discount_num</c> 为千分制折扣数（5.1 折填 5100）；官方对 <c>discount_fee</c>（≤200 元）与折扣下限（≥2 折）有硬校验。
    /// </para>
    /// <para><b>新建券状态恒为 1（编辑中）</b>：官方状态机要求生效须另行调用 <see cref="UpdateCouponStatusAsync"/>。</para>
    /// </remarks>
    [Post("/shop/coupon/add")]
    Task<WxaResponse> AddCouponAsync(
        [Body] WxaShopCouponAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新优惠券信息（<c>POST /shop/coupon/update</c>，官方原文「当且仅当优惠券状态为 1 时可更新」）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/coupon/update_coupon.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>coupon</c> 包装，字段集与添加侧同源并额外支持 <c>coupon.status</c>），见 <see cref="WxaShopCouponUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体 <c>{"coupon":{…}}</c>，与 <see cref="AddCouponAsync"/> <b>同包装同名</b>
    /// （两页参数表逐行一致，仅更新侧多出选填的 <c>coupon.status</c>）⇒ 路由末段是 <c>update</c>（不是 <c>update_coupon</c>），
    /// 守卫 SC1 锁定，勿按官方文档文件名 <c>update_coupon.html</c> 拼路由。
    /// </para>
    /// <para>
    /// <b>覆盖式全量写</b>：<c>coupon</c> 整份替换，漏传字段即失去原值 ⇒ 必须先 <see cref="GetCouponAsync"/> 读回再改。
    /// 生效（状态 2）之后本端点<b>不可用</b>，官方只留库存与状态两支写入口。
    /// </para>
    /// <para><b>状态取值（官方枚举-status）</b>：1 未生效编辑中 / 2 生效 / 3 已过期 / 4 已作废；
    /// 本端点的 <c>coupon.status</c> 为选填，改状态的主入口是 <see cref="UpdateCouponStatusAsync"/>。</para>
    /// <para><b>金额单位分、折扣千分制</b>与 <see cref="AddCouponAsync"/> 同一口径；<c>discount_fee</c> ≤ 200 元、折扣不得低于 2 折。</para>
    /// </remarks>
    [Post("/shop/coupon/update")]
    Task<WxaResponse> UpdateCouponAsync(
        [Body] WxaShopCouponUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取优惠券信息（<c>POST /shop/coupon/get</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/coupon/get_coupon.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>out_coupon_id</c> 一支，必填），见 <see cref="WxaShopCouponGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>result.coupon</c> 两层的单券详情，见 <see cref="WxaShopCouponGetResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体 <c>{"out_coupon_id":"…"}</c>；本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>应答是 <c>result</c> 包装</b>：官方回包为 <c>{errcode, result:{coupon:{…}}}</c> ⇒ 详情不在顶层，
    /// 与 <see cref="GetCouponListAsync"/> 的顶层 <c>total_num</c> + <c>result_list</c>（<b>无</b> result 包装）形态不同，
    /// 两个应答类不得互抄（守卫 SC5 锁定）。
    /// </para>
    /// <para>
    /// <b>详情应答不含库存</b>：<c>coupon_stock</c>（<c>issued_num</c> / <c>receive_num</c>）只在
    /// <see cref="GetCouponListAsync"/> 的条目里出现 ⇒ 单查取不到余量，宿主若需库存口径必须走列表或自行记账。
    /// </para>
    /// <para><b>回显字段</b>：详情额外给出 <c>status</c> / <c>create_time</c> / <c>update_time</c> / <c>appid</c>（时间戳为秒级）。</para>
    /// </remarks>
    [Post("/shop/coupon/get")]
    Task<WxaShopCouponGetResponse> GetCouponAsync(
        [Body] WxaShopCouponGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取优惠券列表（<c>POST /shop/coupon/get_list</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/coupon/get_coupon_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>page_size</c> 必填、<c>offset</c> 选填），见 <see cref="WxaShopCouponGetListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>total_num</c> + <c>result_list[]</c>（每项含 <c>coupon</c> 与可选 <c>coupon_stock</c>），见 <see cref="WxaShopCouponGetListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；翻页口径为<b>偏移式</b> <c>offset</c>（官方原文「从 0 开始」）
    /// + <c>page_size</c>（官方原文「必填，最大为 200」）⇒ 传 0 是第一页，超限按官方参数校验失败处理，不得凭记忆放宽。
    /// </para>
    /// <para>
    /// <b>条目是「券 + 库存」的双包装对象</b>：<c>result_list[]</c> 元素形如
    /// <c>{coupon:{…}, coupon_stock:{…}}</c>，<c>coupon_stock</c> <b>可能整体缺省</b>（官方示例第 2 项即无该键）
    /// ⇒ 消费侧必须容忍 null，不得假定每行都有库存。
    /// </para>
    /// <para>
    /// <b>官方示例 JSON 自身畸形</b>（留档，勿据示例建模）：<c>get_coupon_list</c> 回包示例里
    /// <c>"coupon_stock": {</c> 一处缺引号且少一层闭合括号，而随后的<b>回包参数说明表</b>写的是
    /// <c>coupon.coupon_stock.*</c> ⇒ 本仓按「条目内并列的两支键」建模，即
    /// <see cref="WxaShopCouponListItem.Coupon"/> 与 <see cref="WxaShopCouponListItem.CouponStock"/>。
    /// </para>
    /// <para>
    /// <b>示例独有键</b>：<c>coupon_info.ext_info.valid_time</c>（秒级时间戳）只出现在本页回包示例、参数表未列
    /// ⇒ 建模为可空字段并在注释留档，请求侧不填（WhenWritingNull 自动省略）。
    /// </para>
    /// <para><b>库存字段语义反直觉</b>：<c>stock_info.issued_num</c> 官方说明是「优惠券库存<b>剩余量</b>」、
    /// <c>receive_num</c> 是「优惠券<b>发放量</b>」⇒ 名称与直觉相反（issued 不是发放量），守卫 SC5 以字段名 + 注释双锁。</para>
    /// </remarks>
    [Post("/shop/coupon/get_list")]
    Task<WxaShopCouponGetListResponse> GetCouponListAsync(
        [Body] WxaShopCouponGetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新优惠券库存（<c>POST /shop/coupon/update_coupon_stock</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/coupon/update_coupon_stock.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>coupon_stock</c> 包装，三支键全必填），见 <see cref="WxaShopCouponStockUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体 <c>{"coupon_stock":{out_coupon_id, stock_info:{issued_num, receive_num}}}</c>，
    /// 参数表三支键<b>全部必填</b>（含 <c>stock_info.receive_num</c>）⇒ 只改剩余量也必须把发放量一并回传当前值。
    /// </para>
    /// <para>
    /// <b>覆盖式全量写、非增量</b>：<c>issued_num</c>（剩余量）/ <c>receive_num</c>（发放量）都是<b>绝对值</b>
    /// ⇒ 并发扣减场景下「读-改-写」存在竞态，宿主须自行串行化或以回调事件重算，不得把本端点当 <c>+N</c> 增量接口用。
    /// </para>
    /// <para>
    /// <b>与 <c>receive_info.total_num</c> 分属两套口径</b>：券体里的 <c>total_num</c> 官方明写「只做展示」，
    /// 本端点的 <c>stock_info</c> 才是实际记账面 ⇒ 两者互不换算，改了本端点不会同步券体展示值。
    /// </para>
    /// <para><b>路由名照官方原文</b>：<c>update_coupon_stock</c>（带 <c>coupon_</c> 段），与状态支 <c>update_status</c> 形成对照，守卫 SC1 锁定。</para>
    /// </remarks>
    [Post("/shop/coupon/update_coupon_stock")]
    Task<WxaResponse> UpdateCouponStockAsync(
        [Body] WxaShopCouponStockUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新优惠券状态（<c>POST /shop/coupon/update_status</c>，官方状态机单向不可逆）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/coupon/update_coupon_status.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>out_coupon_id</c> + <c>status</c> 两支），见 <see cref="WxaShopCouponStatusUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体 <c>{"out_coupon_id":"…","status":2}</c>；
    /// <b>本页官方没有「请求参数说明」表</b>（只有请求示例与枚举-status 表）⇒ 字段集取自请求示例，已在此留档（勿按其它页补数）。
    /// </para>
    /// <para>
    /// <b>状态机不可逆</b>（官方原文）：「优惠券初始状态为 1，更新为 2 后<b>不可回退为 1</b>。状态 3、4 <b>只可由状态 2 更新</b>」
    /// ⇒ 本端点是<b>单向门</b>：误置 4（已作废）无法用本域任何端点撤销，官方亦无删除端点。宿主须自行加二次确认，
    /// 且本端点<b>不得</b>放进自动重试管道。
    /// </para>
    /// <para><b>路由末段是 <c>update_status</c></b>（官方接口地址原文），与文档文件名 <c>update_coupon_status.html</c> 不同形 ⇒ 守卫 SC1 按路由锁定。</para>
    /// <para><b>状态取值</b>：1 未生效编辑中 / 2 生效 / 3 已过期 / 4 已作废 —— 与用户券侧的 100~103 是<b>两套值域</b>，不得互用（见 <see cref="IWxaShopUserCouponService"/>）。</para>
    /// </remarks>
    [Post("/shop/coupon/update_status")]
    Task<WxaResponse> UpdateCouponStatusAsync(
        [Body] WxaShopCouponStatusUpdateRequest request,
        CancellationToken cancellationToken = default);
}
