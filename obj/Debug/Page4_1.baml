   M S B A M L   `   `   `  ����I  EVeryBigPipirka, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:VeryBigPipirkaVeryBigPipirka  5          VeryBigPipirka.Page4_1   5      _ [MaterialDesignThemes.Wpf, Version=5.0.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .localclr-namespace:VeryBigPipirka     5        N� Title$  Page4_1��5
      .��5      � .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ Products��5      $��10,10,200,20q�6%      AutoGenerateColumns .     CanUserAddRows .     
IsReadOnly .    Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$% Категория товара��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6?   �� *ProductCategory.CategoryName+  5      $# Название товара��6%    6>   �� *ProductName+  5      $# Описание товара��6%    6>   �� *DescriptionProducts+  5      $$ Товара на складе��6%    6?   �� *QuantityInStock+  5      $ Фото товара��6%    6:   �� *ProductImage.ImagePath+  5      $ Цена товара��6%    6:   �� *PriceHistory.NewPrice+5      �� 5   
   -   5         ��Name$
 edit��5      $	��185��6   $	��Top=�6+   $��585,245,0,0q�6C   $��35��6X   $
��Left=�6d   $��16(�6   $��Изменить��6�   $%��Bahnschrift Light SemiCondensed)�6�   6�   �� 5   
   -   5        $ create��5      $��Добавить��6!   $
��Left=�64   $��35��6O   $��585,325,0,0q�6[   $	��Top=�6p   $	��185��6�   $��16(�6�   $%��Bahnschrift Light SemiCondensed)�6�   6�   �� 5   
   -   5        $ delete��5      $��Удалить��6!   $
��Left=�63   $��35��6N   $��585,285,0,0q�6Z   $	��Top=�6o   $	��185��6�   	 J� RenderTransformOrigin$	 0.307,0.408&�6�   $��16(�6�   $%��Bahnschrift Light SemiCondensed)�6�   6�   �� 5   
   -   5        
 ��Name$
 Three��5      $
��Left=�6!   $��29��6<   $��
585,78,0,0q�6H    �� TextWrapping$
 Wrap=�6\   $	��Top=�6p   $	��185��6�   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Столбец №3��6�    � Cursor$  ��6�   $%��Bahnschrift Light SemiCondensed)�6�   �� 5   
   -   5        $	
 Two��5      $
��Left=�6   $��29��6:   $��
585,44,0,0q�6F   $
 Wrap=�6Z   $	��Top=�6n   $	��185��6�   $ Столбец №2��6�   $  ��6�   $%��Bahnschrift Light SemiCondensed)�6�   �� 5   
   -   5        $

 Five��5      $
��Left=�6    $��29��6;   $��585,112,0,0q�6G   $
 Wrap=�6\   $	��Top=�6p   $	��185��6�   $ Столбец №4��6�   $  ��6�   $%��Bahnschrift Light SemiCondensed)�6�   �� 5   
   -   5         ��Name$	 One��5      $��585,10,10,0q�6    $	��Top=�65   $��29��6M   $ Столбец №1��6Y   $  ��6�   $%��Bahnschrift Light SemiCondensed)�6�   �� 5   
   -	   5        $	 Six��5      $��585,146,10,0q�6    $	��Top=�66   $��29��6N   $ Столбец №5��6Z   $  ��6�   $%��Bahnschrift Light SemiCondensed)�6�   �� 5   
   -
   5        $ Seven��5      $��585,180,10,0q�6"   $	��Top=�68   $��29��6P   $ Столбец №6��6\   $  ��6�   $%��Bahnschrift Light SemiCondensed)�6�   5       5!      