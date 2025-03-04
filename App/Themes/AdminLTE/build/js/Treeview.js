/**
 * --------------------------------------------
 * AdminLTE Treeview.js
 * License MIT
 * --------------------------------------------
 */

const Treeview = (($) => {
  /**
   * Constants
   * ====================================================
   */

  const NAME               = 'Treeview'
  const DATA_KEY           = 'lte.treeview'
  const EVENT_KEY          = `.${DATA_KEY}`
  const JQUERY_NO_CONFLICT = $.fn[NAME]

  const Event = {
    SELECTED     : `selected${EVENT_KEY}`,
    EXPANDED     : `expanded${EVENT_KEY}`,
    COLLAPSED    : `collapsed${EVENT_KEY}`,
    LOAD_DATA_API: `load${EVENT_KEY}`
  }

  const Selector = {
    LI           : '.nav-item',
    LINK         : '.nav-link',
    TREEVIEW_MENU: '.nav-treeview',
    OPEN         : '.menu-open',
    DATA_WIDGET  : '[data-widget="treeview"]'
  }

  const ClassName = {
    LI           : 'nav-item',
    LINK         : 'nav-link',
    TREEVIEW_MENU: 'nav-treeview',
    OPEN         : 'menu-open'
  }

  const Default = {
    trigger       : `${Selector.DATA_WIDGET} ${Selector.LINK}`,
    animationSpeed: 300,
    accordion     : true
  }

  /**
   * Class Definition
   * ====================================================
   */
  class Treeview {
    constructor(element, config) {
      this._config  = config
      this._element = element
    }

    // Public

    init() {
      this._setupListeners()
    }

    expand(treeviewMenu, parentLi) {
      const expandedEvent = $.Event(Event.EXPANDED)

      if (this._config.accordion) {
        const openMenuLi   = parentLi.siblings(Selector.OPEN).first()
        const openTreeview = openMenuLi.find(Selector.TREEVIEW_MENU).first()
        this.collapse(openTreeview, openMenuLi)
      }

      treeviewMenu.slideDown(this._config.animationSpeed, () => {
        parentLi.addClass(ClassName.OPEN)
        $(this._element).trigger(expandedEvent)
      })
    }

    collapse(treeviewMenu, parentLi) {
      const collapsedEvent = $.Event(Event.COLLAPSED)

      treeviewMenu.slideUp(this._config.animationSpeed, () => {
        parentLi.removeClass(ClassName.OPEN)
        $(this._element).trigger(collapsedEvent)
        treeviewMenu.find(`${Selector.OPEN} > ${Selector.TREEVIEW_MENU}`).slideUp()
        treeviewMenu.find(Selector.OPEN).removeClass(ClassName.OPEN)
      })
    }

    toggle(event) {
      const $relativeTarget = $(event.currentTarget)
      const treeviewMenu    = $relativeTarget.next()

      if (!treeviewMenu.is(Selector.TREEVIEW_MENU)) {
        return
      }

      event.preventDefault()

      const parentLi = $relativeTarget.parents(Selector.LI).first()
      const isOpen   = parentLi.hasClass(ClassName.OPEN)

      if (isOpen) {
        this.collapse($(treeviewMenu), parentLi)
      } else {
        this.expand($(treeviewMenu), parentLi)
      }
    }

    // Private

    _setupListeners() {
      $(document).on('click', this._config.trigger, (event) => {
        this.toggle(event)
      })
    }

    // Static

    static _jQueryInterface(config) {
      return this.each(function () {
        let data      = $(this).data(DATA_KEY)
        const _config = $.extend({}, Default, $(this).data())

        if (!data) {
          data = new Treeview($(this), _config)
          $(this).data(DATA_KEY, data)
        }

        if (config === 'init') {
          data[config]()
        }
      })
    }
  }

  /**
   * Data API
   * ====================================================
   */

  $(window).on(Event.LOAD_DATA_API, () => {
    $(Selector.DATA_WIDGET).each(function () {
      Treeview._jQueryInterface.call($(this), 'init')
    })
  })

  /**
   * jQuery API
   * ====================================================
   */

  $.fn[NAME] = Treeview._jQueryInterface
  $.fn[NAME].Constructor = Treeview
  $.fn[NAME].noConflict  = function () {
    $.fn[NAME] = JQUERY_NO_CONFLICT
    return Treeview._jQueryInterface
  }

  return Treeview
})(jQuery)

export default Treeview