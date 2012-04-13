﻿using System.Web.Mvc;

namespace Zephyr.Web.Mvc
{
    /// <summary>
    /// ViewDataContainer class.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public class ViewDataContainer<TModel>
        : IViewDataContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewDataContainer&lt;TModel&gt;"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public ViewDataContainer(TModel model)
        {
            ViewData = new ViewDataDictionary<TModel>(model);
        }

        /// <summary>
        /// Gets or sets the view data dictionary.
        /// </summary>
        /// <value></value>
        /// <returns>The view data dictionary.</returns>
        public ViewDataDictionary ViewData { get; set; }
    }
}