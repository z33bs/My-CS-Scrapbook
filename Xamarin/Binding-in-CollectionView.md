
So the issue with Command binding within the collectionview is that the binding context is the element itself, not the viewmodel. 
So in the example below, each Item will be the binding context. To bind to a command in the ViewModel, you need to get its path. Two ways to do this:
1) Name the CollectionView. Then bind to it's BindingContext (which will be the ViewModel).

```xaml
<CollectionView x:Name="ItemsCollectionView" ItemsSource="{Binding Items}">
<CollectionView.ItemTemplate>
<DataTemplate>
  <StackLayout Padding="10" InputTransparent="False">
    <StackLayout.GestureRecognizers>
	    <TapGestureRecognizer Command="{Binding BindingContext.OnItemSelectedCommand
        , Source={x:Reference Name=ItemsCollectionView}}" 
  			CommandParameter="{Binding .}"/>
    </StackLayout.GestureRecognizers>

  <Label Text="{Binding Text}" d:Text="{Binding .}" LineBreakMode="NoWrap" Style="{DynamicResource ListItemTextStyle}" FontSize="16" InputTransparent="True" />
  <Label Text="{Binding Description}" d:Text="Item descripton" LineBreakMode="NoWrap" Style="{DynamicResource ListItemDetailTextStyle}" FontSize="13" InputTransparent="True"/>
  </StackLayout>                  
</DataTemplate>

</CollectionView.ItemTemplate>
</CollectionView>
```

or 2) Use AncestorType and select the ViewModel type

```xaml
<... xmlns:vm="clr-namespace:TestMyMvvm.ViewModels" .../>
<TapGestureRecognizer Command="{Binding 
  Source={RelativeSource AncestorType={x:Type vm:ItemsViewModel}}
  , Path=OnItemSelectedCommand}" 
  CommandParameter="{Binding .}"/>
```
