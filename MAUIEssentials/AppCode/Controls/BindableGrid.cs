namespace MAUIEssentials.AppCode.Controls
{
    public class BindableGrid : Grid
    {
        public static readonly BindableProperty ColumnCountProperty =
            BindableProperty.Create(nameof(ColumnCount), typeof(int), typeof(BindableGrid), 1);

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<object>), typeof(BindableGrid), null);

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(BindableGrid), null);

        public int ColumnCount
        {
            get => (int)GetValue(ColumnCountProperty);
            set => SetValue(ColumnCountProperty, value);
        }

        public IEnumerable<object> ItemsSource
        {
            get => (IEnumerable<object>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            CreateGrid();
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == ItemsSourceProperty.PropertyName)
            {
                CreateGrid();
            }

            base.OnPropertyChanged(propertyName);
        }

        private void CreateGrid()
        {
            // Check for data
            if (ItemsSource == null || ItemsSource.Count() == 0)
            {
                return;
            }

            // Create the grid
            RowDefinitions = CreateRowDefinitions();

            if (!ColumnDefinitions.Any())
            {
                ColumnDefinitions = CreateColumnDefinitions();
            }

            CreateCells();
        }

        private RowDefinitionCollection CreateRowDefinitions()
        {
            var rowDefinitions = new RowDefinitionCollection();

            int rowCount = (int)Math.Ceiling((double)ItemsSource.Count() / ColumnCount);

            for (int i = 0; i < rowCount; i++)
            {
                    rowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            return rowDefinitions;
        }

        private ColumnDefinitionCollection CreateColumnDefinitions()
        {
            var columnDefinitions = new ColumnDefinitionCollection();

            for (int i = 0; i < ColumnCount; i++)
            {
                columnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            return columnDefinitions;
        }

        private void CreateCells()
        {
            int rowIndex = 0, colIndex = 0;

            // Clear existing children
            if (Children != null)
            {
                Children.Clear();
            }

            foreach (var item in ItemsSource)
            {
                // Create the view for the current item
                var cellView = CreateCellView(item);

                // Set the row and column for the view
                Grid.SetRow(cellView, rowIndex);
                Grid.SetColumn(cellView, colIndex);

                // Add the view to the grid
                Children.Add(cellView);

                // Update column and row indices
                if (colIndex == ColumnCount - 1)
                {
                    colIndex = 0;
                    rowIndex++;
                }
                else
                {
                    colIndex++;
                }
            }
        }

        private View CreateCellView(object item)
        {
            var view = (View)ItemTemplate.CreateContent();
            var bindableObject = (BindableObject)view;

            if (bindableObject != null)
            {
                bindableObject.BindingContext = item;
            }

            return view;
        }
    }
}