   M S B A M L   `   `   `  ���� B  >AISDE_1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:AISDE_1AISDE_1       AISDE_1.GraphDisplayWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:AISDE_1       � Title$  GraphDisplayWindow��$	��768��$
��1200��.��� -    �Name$ 	graphGrid��$
��Left=�$
��1200��$	��768��$	��Top=�.+��� -    ��Name$ canvas��$
��Left=�$	��724��$	��Top=�$
��1190��.+��� -    ��Name$ 	mstButton��$	��MST�� �� Left$ 25��
 �� Top$ 35��$��75���� -   $ 
pathButton��$��Dijkstra��$ 25��$ 67��$��75���� -   $ floydPathButton��$��Floyd��$ 25��$	 100��$��75���� -   $ randomCostsButton��$��Losowe wagi��$ 25��$	 135��$��75���� -    ��Name$ StartingSolutionTextBlock��$
 1062��$
��Wrap=�$��0��$ 22��$��60���� -    ��Name$ StartingSolutionLabel��$'��!Koszt rozwiązania początkowego:��$	 832��$ 17��$	��195���� -	   $ CalculatedSolutionLabel��$*��$Koszt rozwiązania optymalizowanego:��$	 832��$ 43���� -
   $ CalculatedSolutionTextBox��$
 1062��$
��Wrap=�$ 48��$��67��$��0���� -   $ IterationsLabel��$��Liczba iteracji:��$	 832��$ 67���� -   $ IterationsTextBlock��$
 1062��$
��Wrap=�$ 74��$��0���� -   $ AcceptedLabel��$!��Zaakceptowane rozwiązania:��$	 832��$ 93���� -   $ AcceptedTextBlock��$
 1062��$
��Wrap=�$��0��$ 98��